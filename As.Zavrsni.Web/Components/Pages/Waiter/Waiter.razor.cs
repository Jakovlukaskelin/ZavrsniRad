using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Products.Model;
using As.Zavrsni.Aplication.Products.Query;
using As.Zavrsni.Domain.Entites;
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
        public int OrderQuantity { get; set; } = 1;
        private SfToast toast;
        private string toastMessage;
        public List<OrderItem> orderItems { get; set; } = new List<OrderItem>();

        private HubConnection hubConnection;

        private string errorMessage;

        protected override async Task OnInitializedAsync()
        {
            
            Products = await Mediator.Send(new GetProductListQuery());
           
            filteredProducts = Products;
            hubConnection = new HubConnectionBuilder().WithUrl(NavigationManager.ToAbsoluteUri("/orderhub")).Build();
            hubConnection.On<string>("ReceiveOrderUpdate", (message) =>
            {
                ShowOrderNotification(message);
            });

            await hubConnection.StartAsync();
        }

        private void ShowOrderNotification(string message)
        {
            toastMessage = $"Order Update: {message}";
            toast.ShowAsync(new ToastModel { Title = "Order Update", Content = toastMessage });
            
        }

        private void OnProductTypeChange(string productType)
        {
            selectedProductType = productType;
            FilterProducts();
        }

        private void FilterProducts()
        {
            if (selectedProductType == "Hrana")
            {
                filteredProducts = Products.Where(p => p.ProductType == "Hrana").OrderBy(x => x.ExpiryDate).ToList();
            }
            else if (selectedProductType == "Pica")
            {
                filteredProducts = Products.Where(p => p.ProductType != "Hrana").ToList();
            }
            else
            {
                filteredProducts = Products;
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

                        _context.Consumptions.Add(consumption);
                        _context.Products.Update(product);
                    }
                }
                bool allHrana = orderItems.All(item =>
                {
                    var product = Products.FirstOrDefault(p => p.ProductName == item.ProductName);
                    return product != null && product.ProductType == "Hrana";
                });
                var orderDetails = string.Join(", ", orderItems.Select(i => $"{i.ProductName} (x{i.Quantity})"));
                orderItems.Clear();
                FilterProducts();
               
                await _context.SaveChangesAsync();
                await LoadProducts();
                StateHasChanged();
                if (allHrana)
                {
                    await hubConnection.SendAsync("SendOrderUpdate", $"New Hrana order submitted: {orderDetails}");
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
            toastMessage = $"Low stock alert: {productName} only has {quantity} items left.";
            toast.ShowAsync(new ToastModel { Title = "Low Stock Alert", Content = toastMessage });
        }

        private void Logout()
        {
            NavigationManager.NavigateTo("/");
        }
    }

    public class OrderItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
