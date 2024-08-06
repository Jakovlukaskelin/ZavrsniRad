using As.Zavrsni.Aplication.Products.Model;
using As.Zavrsni.Aplication.Products.Query;
using MediatR;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Inputs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace As.Zavrsni.Web.Components.Pages.Waiter
{
    public partial class Waiter
    {
        [Inject]
        public IMediator Mediator { get; set; }

        public List<ProductsModel> Products { get; set; } = new List<ProductsModel>();
        public List<ProductsModel> filteredProducts { get; set; } = new List<ProductsModel>();
        public List<string> productTypes { get; set; } = new List<string>();
        public string selectedProductType { get; set; }
        public ProductsModel selectedProduct { get; set; }
        public int orderQuantity { get; set; } = 1;

        public List<OrderItem> orderItems { get; set; } = new List<OrderItem>();

        protected override async Task OnInitializedAsync()
        {
            Products = await Mediator.Send(new GetProductListQuery());
            productTypes = Products.Select(p => p.ProductType).Distinct().ToList();
            filteredProducts = Products;
        }

        private void OnProductTypeChange(ChangeEventArgs<string> args)
        {
            selectedProductType = args.Value;
            FilterProducts();
        }

        private void FilterProducts()
        {
            if (!string.IsNullOrEmpty(selectedProductType))
            {
                filteredProducts = Products.Where(p => p.ProductType == selectedProductType).ToList();
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
    }

    public class OrderItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
