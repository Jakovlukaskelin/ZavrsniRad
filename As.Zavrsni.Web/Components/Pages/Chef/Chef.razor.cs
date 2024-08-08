using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Products.Model;
using As.Zavrsni.Aplication.Products.Query;
using As.Zavrsni.Domain.Entites;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
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
            CheckExpiringProducts();
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

        private async Task LoadProducts()
        {
            Products = await Mediator.Send(new GetProductListQuery());
            Products = Products.Where(p => p.ProductType == "Hrana").ToList();
            CheckExpiringProducts();
        }
        private void CheckExpiringProducts()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var expiringProducts = Products.Where(p => p.ExpiryDate.HasValue &&
                                                       (p.ExpiryDate.Value.ToDateTime(new TimeOnly()) - today.ToDateTime(new TimeOnly())).TotalDays <= 2)
                                           .ToList();

            if (expiringProducts.Any())
            {
                var content = "The following products are nearing their expiry date: " + string.Join(", ", expiringProducts.Select(p => p.ProductName));
                ShowExpiringProductsToast(content);
            }

            foreach (var product in Products.Where(p => p.Quantity <= 10)) 
            {
                ShowToast(product.ProductName, product.Quantity);
            }
        }
        private void ShowExpiringProductsToast(string content)
        {
            var toastMessage = content;
            ToastObj.ShowAsync(new ToastModel { Title = "Expiring Products Alert", Content = toastMessage });
        }
        private void ShowToast(string productName , int quantity)
        {
            toastMessage = $"Low stock alert: {productName} only has {quantity} items left.";
            ToastObj.ShowAsync(new ToastModel { Title = "Low Stock Alert", Content = toastMessage });
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
            CheckExpiringProducts();
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
    }
}
