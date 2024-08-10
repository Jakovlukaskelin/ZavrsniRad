using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Notifikacije.Model;
using As.Zavrsni.Aplication.Notifikacije.Query;
using As.Zavrsni.Aplication.Products.Model;
using As.Zavrsni.Aplication.Products.Query;
using As.Zavrsni.Domain.Entites;

using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Notifications;

namespace As.Zavrsni.Web.Components.Pages.Chef
{
    public partial class Chef
    {
        [Inject]
        public IMediator Mediator { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        public IZavrsniDbContext _context { get; set; }
        public List<ProductsModel> Products { get; set; } = new List<ProductsModel>();
        public List<NotificationModel> Notifications { get; set; } = new List<NotificationModel>();

        private bool IsNotificationVisible = false;

        private List<NotificationModel> NotificationModel = new List<NotificationModel>();

        private bool IsEditProductDialogVisible = false;

        private ProductsModel EditProductModel = new ProductsModel();
        private int UnreadNotificationsCount => NotificationModel.Count(n => !n.Status);

        private SfToast ToastObj;
        private string toastMessage;
        private bool IsAddProductDialogVisible = false;
        private ProductsModel NewProduct = new ProductsModel();
        private HubConnection hubConnection;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Products = await Mediator.Send(new GetProductListQuery());
            Products = Products.Where(p => p.ProductType == "Hrana").ToList();
            Notifications = await Mediator.Send(new GetNotificationQuery());
            await CheckExpiringProducts();

            hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/orderHub"))
            .Build();

            hubConnection.On<string>("ReceiveOrderUpdate", async (message) =>
            {
                ShowOrderNotification(message);
                await LoadProducts();
                await InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }

        private void ShowOrderNotification(string message)
        {
            toastMessage = $"Order Update: {message}";
            ToastObj.ShowAsync(new ToastModel { Title = "Order Update", Content = toastMessage });
        }

        public void Dispose()
        {
            _ = hubConnection?.DisposeAsync();
        }
        private void NavigateToNotificationsPage()
        {
            NavigationManager.NavigateTo("/notifications");
        }
        private async Task LoadProducts()
        {
            Products = await Mediator.Send(new GetProductListQuery());
            Products = Products.Where(p => p.ProductType == "Hrana").ToList();
            await CheckExpiringProducts();
        }
        private async Task CheckExpiringProducts()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var expiringOrLowStockProducts = Products.Where(p =>
                (p.ExpiryDate.HasValue && (p.ExpiryDate.Value.ToDateTime(new TimeOnly()) - today.ToDateTime(new TimeOnly())).TotalDays <= 3) ||
                (p.Quantity <= 10))
                .ToList();

            foreach (var product in expiringOrLowStockProducts)
            {
                string message = product.ExpiryDate.HasValue && (product.ExpiryDate.Value.ToDateTime(new TimeOnly()) - today.ToDateTime(new TimeOnly())).TotalDays <= 3
                    ? $"Proizvod je blizu istek roka: {product.ProductName}"
                    : $"Količina je niska: {product.ProductName} ima još {product.Quantity} komada.";

                if (!await NotificationExists(product.ProductId, message))
                {
                    var notification = new NotificationModel
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductType = product.ProductType,
                        ExpiryDate = product.ExpiryDate,
                        Quantity = product.Quantity,
                        Message = message,
                        NotificationDate = DateTime.Now
                    };

                    NotificationModel.Add(notification);
                    await AddNotificationToDatabase(notification);
                }
            }

            if (NotificationModel.Any())
            {
                ShowNotificationToast("Imate novu notifikaciju.");
            }
        }
        private async Task<bool> NotificationExists(int productId, string message)
        {
            int retryCount = 3;
            while (retryCount > 0)
            {
                try
                {
                    return await _context.Notifications
                                         .AsNoTracking()
                                         .AnyAsync(n => n.ProductId == productId && n.Message == message && n.Status == "false");
                }
                catch (InvalidOperationException ex)
                {
                    retryCount--;
                    if (retryCount == 0) throw;
                    await Task.Delay(1000);
                }
            }

            return false;
        }
        private async Task AddNotificationToDatabase(NotificationModel notification)
        {
            var entity = new Domain.Entites.Notification
            {
                ProductId = notification.ProductId,
                NotificationDate = notification.NotificationDate,
                Message = notification.Message,
                Status = "false"
            };

            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();
        }
        private void ShowNotificationToast(string message)
        {
            toastMessage = message;
            ToastObj.ShowAsync(new ToastModel { Title = "Notification", Content = toastMessage });
        }

        private void OpenAddProductDialog()
        {
            IsAddProductDialogVisible = true;
        }
        private async Task AddProduct()
        {
            await AddProductToDatabase(NewProduct);
            Products = await Mediator.Send(new GetProductListQuery());
            Products = Products.Where(p => p.ProductType == "Hrana").ToList();

            IsAddProductDialogVisible = false;
            await CheckExpiringProducts();
        }

        private async Task AddProductToDatabase(ProductsModel newProduct)
        {
            var entity = new Product
            {
                ProductName = newProduct.ProductName,
                ProductType = "Hrana",
                ExpiryDate = newProduct.ExpiryDate,
                Quantity = newProduct.Quantity
            };

            _context.Products.Add(entity);
            await _context.SaveChangesAsync();
        }
        private async Task UpdateProductInDatabase(ProductsModel productToUpdate)
        {
            var entity = await _context.Products.FindAsync(productToUpdate.ProductId);
            if (entity != null)
            {
                entity.ProductName = productToUpdate.ProductName;
                entity.ExpiryDate = productToUpdate.ExpiryDate;
                entity.Quantity = productToUpdate.Quantity;

                _context.Products.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
        private void EditProduct(ProductsModel product)
        {
            EditProductModel = new ProductsModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ExpiryDate = product.ExpiryDate,
                Quantity = product.Quantity
            };
            IsEditProductDialogVisible = true;
        }
        private async Task SaveProductChanges()
        {
            await UpdateProductInDatabase(EditProductModel);
            Products = await Mediator.Send(new GetProductListQuery());
            Products = Products.Where(p => p.ProductType == "Hrana").ToList();

            IsEditProductDialogVisible = false;
            await CheckExpiringProducts();
        }
        private void Logout()
        {
            NavigationManager.NavigateTo("/");
        }
        private void MarkAllAsRead()
        {
            foreach (var notification in NotificationModel)
            {
                notification.Status = true;
            }
            IsNotificationVisible = false;
        }
        private void ToggleNotificationVisibility()
        {
            IsNotificationVisible = !IsNotificationVisible;
        }
    }
}
