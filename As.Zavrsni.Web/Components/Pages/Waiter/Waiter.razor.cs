﻿using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Model;
using As.Zavrsni.Aplication.Model.Query;
using As.Zavrsni.Aplication.Notifikacije.Model;
using As.Zavrsni.Aplication.Notifikacije.Query;
using As.Zavrsni.Aplication.Orders;
using As.Zavrsni.Aplication.Orders.NewFolder.Query;
using As.Zavrsni.Aplication.Products.Model;
using As.Zavrsni.Aplication.Products.Query;
using As.Zavrsni.Domain.Entites;
using As.Zavrsni.Web.Components.Pages.Notification;
using Elastic.Clients.Elasticsearch;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Notifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace As.Zavrsni.Web.Components.Pages.Waiter
{
    public partial class Waiter
    {
        [Inject]
        public IMediator Mediator { get; set; }

        [Inject]
        public IZavrsniDbContext _context { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        public List<ProductsModel> Products { get; set; } = new List<ProductsModel>();
        public List<ProductsModel> filteredProducts { get; set; } = new List<ProductsModel>();
        public string selectedProductType { get; set; }
        public ProductsModel selectedProduct { get; set; }
        public List<NotificationModel> Notifications { get; set; } = new List<NotificationModel>();
        private int UnreadNotificationsCount => Notifications.Count(n => !n.Status);
        public int OrderQuantity { get; set; } = 1;
        private SfToast toast;
        private string toastMessage;
        public List<OrderItem> orderItems { get; set; } = new List<OrderItem>();
        private List<OrderModel> Orders;
        private HubConnection hubConnection;
        private string errorMessage;
        private List<ProductsModel> DuplicateProducts = new List<ProductsModel>();
        private bool isWarningDialogVisible = false;
        
      
        protected override async Task OnInitializedAsync()
        {
            
            Products = await Mediator.Send(new GetProductListQuery());
            filteredProducts = Products;
            Notifications = await Mediator.Send(new GetNotificationQuery());
            Orders = await Mediator.Send(new GetOrdersQuery());
            await CheckLowStockProducts();
            hubConnection = new HubConnectionBuilder().WithUrl(NavigationManager.ToAbsoluteUri("/orderhub")).Build();
            hubConnection.On<string>("ReceiveOrderUpdate", (message) =>
            {
                ShowOrderNotification(message);
            });

            await hubConnection.StartAsync();
        }

        private void ShowOrderNotification(string message)
        {
            toastMessage = $"Narudžba: {message}";
            toast.ShowAsync(new ToastModel { Title = "Narudžba", Content = toastMessage });

        }

        private void OnProductTypeChange(string productType)
        {
            selectedProductType = productType;
            FilterProducts();
        }

        private async void FilterProducts()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (selectedProductType == "Hrana")
            {
                filteredProducts = Products
                    .Where(p => p.ProductType == "Hrana" && p.Quantity > 0 && p.ExpiryDate.HasValue && p.ExpiryDate.Value >= today)
                    .GroupBy(p => p.ProductName)
                    .Select(g =>
                    {
                        var orderedProducts = g.OrderBy(p => p.ExpiryDate).ToList();
                        return orderedProducts.First();
                    })
                    .OrderBy(x => x.ExpiryDate)
                    .ToList();
            }
            else if (selectedProductType == "Pica")
            {
                filteredProducts = Products
                    .Where(p => p.ProductType != "Hrana" && p.Quantity > 0)
                    .ToList();
            }
            else
            {
                filteredProducts = Products
                    .Where(p => p.Quantity > 0)
                    .ToList();
            }

            StateHasChanged();
        }

        private void SelectProduct(ProductsModel product)
        {
            selectedProduct = product;
            OrderQuantity = 1;
        }

        private void AddProductToOrder()
        {
            if (selectedProduct != null && OrderQuantity > 0 && OrderQuantity <= selectedProduct.Quantity)
            {
                var existingItem = orderItems.FirstOrDefault(i => i.ProductName == selectedProduct.ProductName);
                if (existingItem != null)
                {
                    existingItem.Quantity += OrderQuantity;
                }
                else
                {
                    orderItems.Add(new OrderItem
                    {
                        ProductName = selectedProduct.ProductName,
                        Quantity = OrderQuantity
                    });
                }
                selectedProduct.Quantity -= OrderQuantity;

                // Check for low stock and show notification
                if (selectedProduct.Quantity <= 10)
                {
                    ShowLowStockNotification(selectedProduct.ProductName, selectedProduct.Quantity);
                }

                selectedProduct = null;
                OrderQuantity = 1;
                FilterProducts();
                StateHasChanged();
            }
        }

        private void RemoveProductFromOrder(OrderItem item)
        {
            var product = Products.FirstOrDefault(p => p.ProductName == item.ProductName);
            if (product != null)
            {
                product.Quantity += item.Quantity;
            }
            orderItems.Remove(item);
            FilterProducts();
            StateHasChanged();
        }

        private async Task SubmitOrder()
        {
            errorMessage = string.Empty;
            try
            {
                if (orderItems == null || !orderItems.Any())
                {
                    throw new Exception("No items in the order to submit.");
                }

                foreach (var item in orderItems)
                {
                    if (item == null)
                    {
                        continue; // Skip null items
                    }

                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == item.ProductName);
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == u.Username);
                    if (product != null)
                    {
                        if (string.IsNullOrEmpty(product.ProductType))
                        {
                            throw new InvalidOperationException("Product type is null or empty.");
                        }

                        product.Quantity -= item.Quantity;

                        var consumption = new Consumption
                        {
                            ProductId = product.ProductId,
                            ConsumptionDate = DateOnly.FromDateTime(DateTime.Now),
                            Quantity = item.Quantity,
                            Type = product.ProductType

                        };
                        var order  = new Order
                        {
                            User  = user,
                            Product = product,
                            ProductId = product.ProductId,
                            OrderDate = DateOnly.FromDateTime(DateTime.Now).ToDateTime(TimeOnly.MaxValue),
                            Quantity = item.Quantity,


                        };
                        _context.Orders.Add(order);
                        _context.Consumptions.Add(consumption);
                        _context.Products.Update(product);
                        await _context.SaveChangesAsync();
                    }
                }
                bool allHrana = orderItems.Any(item =>
                {
                    var product = Products.FirstOrDefault(p => p.ProductName == item.ProductName);
                    return product != null && product.ProductType == "Hrana";
                });
                var orderDetails = string.Join(", ", orderItems.Where(i => Products.Any(p => p.ProductName == i.ProductName && p.ProductType == "Hrana"))
                                            .Select(i => $"{i.ProductName} (x{i.Quantity})"));
                orderItems.Clear();
                FilterProducts();

                await _context.SaveChangesAsync();
                await LoadProducts();
                StateHasChanged();

                if (allHrana)
                {
                    await hubConnection.SendAsync("SendOrderUpdate", $"Narudžba: {orderDetails}");
                }
            }
            catch (InvalidOperationException ex)
            {
                errorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                errorMessage = $"Nema proizvoda za narčit dodajte neki proizvod: {ex.Message}";
            }
        }
        private async Task LoadProducts()
        {
            Products = await Mediator.Send(new GetProductListQuery());

            FilterProducts();
            StateHasChanged();
        }

        private void ShowLowStockNotification(string productName, int quantity)
        {
            toastMessage = $"Niska količina: {productName} ostalo je još {quantity} komada .";
            toast.ShowAsync(new ToastModel { Title = "Niska količina", Content = toastMessage });
        }

        private void Logout()
        {
            NavigationManager.NavigateTo("/");
        }
        private void ShowWarningDialog(string productName)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            
            DuplicateProducts = Products
                .Where(p => p.ProductName == productName && p.ExpiryDate.HasValue && p.ExpiryDate.Value >= today)
                .OrderBy(p => p.ExpiryDate)
                .ToList();

            
            if (DuplicateProducts.Count > 1)
            {
                isWarningDialogVisible = true;
            }
        }
        private void CloseWarningDialog()
        {
            isWarningDialogVisible = false;
        }
        private void NavigateToNotificationsPage()
        {
            NavigationManager.NavigateTo("/notificationadminwaiter");
        }
        private async Task CheckLowStockProducts()
        {
            var lowStockProducts = Products.Where(p => p.Quantity <= 10).ToList();

            foreach (var product in lowStockProducts)
            {
                string message = $"Niska količina: {product.ProductName} ostalo je {product.Quantity} komada.";

                if (!await NotificationExists(product.ProductId, message))
                {
                    var notification = new NotificationModel
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductType = product.ProductType,
                        Quantity = product.Quantity,
                        Message = message,
                        NotificationDate = DateTime.Now
                    };

                    Notifications.Add(notification);
                    await AddNotificationToDatabase(notification);
                }
            }

            if (Notifications.Any())
            {
                ShowNotificationToast("Imaš novu notifikaciju.");
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
                catch (InvalidOperationException)
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
            toast.ShowAsync(new ToastModel { Title = "Notifikacija", Content = toastMessage });
        }
    }

    public class OrderItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
