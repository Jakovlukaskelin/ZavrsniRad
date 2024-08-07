using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Products.Model;
using As.Zavrsni.Aplication.Products.Query;
using As.Zavrsni.Domain.Entites;
using MediatR;
using Microsoft.AspNetCore.Components;
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
        public int orderQuantity { get; set; } = 1;
        private SfToast toast;
        private string toastMessage;
        public List<OrderItem> orderItems { get; set; } = new List<OrderItem>();

        protected override async Task OnInitializedAsync()
        {
            Products = await Mediator.Send(new GetProductListQuery());
            
            filteredProducts = Products;
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
                filteredProducts = Products.Where(p => p.ProductType == "Hrana").ToList();
            }
            else if (selectedProductType == "Drinks")
            {
                filteredProducts = Products.Where(p => p.ProductType != "Hrana").ToList();
            }
            else
            {
                filteredProducts = Products;
            }
        }

        private void SelectProduct(ProductsModel product)
        {
            selectedProduct = product;
            orderQuantity = 1;
        }

        private void AddProductToOrder()
        {
            if (selectedProduct != null && orderQuantity > 0 && orderQuantity <= selectedProduct.Quantity)
            {
                var existingItem = orderItems.FirstOrDefault(i => i.ProductName == selectedProduct.ProductName);
                if (existingItem != null)
                {
                    existingItem.Quantity += orderQuantity;
                }
                else
                {
                    orderItems.Add(new OrderItem
                    {
                        ProductName = selectedProduct.ProductName,
                        Quantity = orderQuantity
                    });
                }
                selectedProduct.Quantity -= orderQuantity;

                // Check for low stock and show notification
                if (selectedProduct.Quantity <= 10)
                {
                    ShowLowStockNotification(selectedProduct.ProductName, selectedProduct.Quantity);
                }

                selectedProduct = null;
                orderQuantity = 1;
                FilterProducts();
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
        }

        private async Task SubmitOrder()
        {
            if (orderItems == null || orderItems.Count == 0)
            {
                throw new InvalidOperationException("No items in the order to submit.");
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
                    if (product.ProductType == null)
                    {
                        throw new InvalidOperationException("Product type is null.");
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

            orderItems.Clear();
            FilterProducts();

            await _context.SaveChangesAsync();
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
