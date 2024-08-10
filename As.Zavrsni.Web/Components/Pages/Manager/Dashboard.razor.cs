﻿using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using MediatR;
using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Products.Model;
using Syncfusion.Blazor.Notifications;
using As.Zavrsni.Aplication.Consumtpion.Model;
using As.Zavrsni.Aplication.Products.Query;
using As.Zavrsni.Aplication.Consumtpion.Query;
using System.Globalization;
using As.Zavrsni.Aplication.StartAndEndDate;
using System.Text.Json;
using As.Zavrsni.Aplication.Common.Enum;
using As.Zavrsni.Domain.Entites;
using Microsoft.EntityFrameworkCore;


namespace As.Zavrsni.Web.Components.Pages.Manager
{
    public partial class Dashboard : IDisposable
    {
        [Inject]
        private NavigationManager NavManager { get; set; }

        private string? currentUrl;

        [Inject]
        public IMediator Mediator { get; set; }

        [Inject]
        public IZavrsniDbContext _context { get; set; }

        public List<ProductsModel> Products { get; set; } = new List<ProductsModel>();
        public List<ConsumptionModel> Consumption { get; set; } = new List<ConsumptionModel>();
        public List<ProductsModel> FilterProducts { get; set; } = new List<ProductsModel>();
        public string SelectedView { get; set; } = "proizvode";

        private ProductsModel newProduct = new ProductsModel();

        private bool isAddingProduct = false;
      
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;

        private ProductsModel selectedProduct;
        private bool isWarningDialogVisible = false;
        private List<ProductsModel> duplicateProducts = new List<ProductsModel>();

        private bool isEditingProduct = false;
        protected override async Task OnInitializedAsync()
        {
           
            Products = await Mediator.Send(new GetProductListQuery());
            Consumption = await Mediator.Send(new GetConsumptionQuery());
            FilterProducts = Products;
            currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.LocationChanged += OnLocationChanged;
            FilterData();
            
        }


        private void Logout()
        {
            NavManager.NavigateTo("/login");
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
            StateHasChanged();
        }

        private void SwitchView(string view)
        {
            SelectedView = view;
            FilterData();
        }

        private void FilterData()
        {

            if (SelectedView == "potrošnja")
            {
                DateOnly startDate = DateOnly.FromDateTime(StartDate);
                DateOnly endDate = DateOnly.FromDateTime(EndDate);

                
                if (StartDate == DateTime.Today && EndDate == DateTime.Today)
                {
                    
                    FilterProducts = Consumption
                        .GroupBy(c => new { c.ProductName, c.ConsumptionDate })
                        .Select(group => new ProductsModel
                        {
                            ProductName = group.Key.ProductName,
                            ProductType = "potrošnja",
                            ExpiryDate = group.Key.ConsumptionDate,
                            Quantity = group.Sum(c => c.Quantity)
                        })
                        .ToList();
                }
                else
                {
                    
                    FilterProducts = Consumption
                        .Where(c => c.ConsumptionDate.HasValue &&
                                    c.ConsumptionDate.Value >= startDate &&
                                    c.ConsumptionDate.Value <= endDate)
                        .GroupBy(c => new { c.ProductName, c.ConsumptionDate })
                        .Select(group => new ProductsModel
                        {
                            ProductName = group.Key.ProductName,
                            ProductType = "potrošnja",
                            ExpiryDate = group.Key.ConsumptionDate,
                            Quantity = group.Sum(c => c.Quantity)
                        })
                        .ToList();
                }
            }
            else
            {
                FilterProducts = Products;
            }

            StateHasChanged();
        }
        
       
        

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
        private void ShowAddProductForm()
        {
            newProduct = new ProductsModel();
            isAddingProduct = true;
        }

        private void CancelAddProduct()
        {
            isAddingProduct = false;
        }
        private async Task AddProductToDatabase()
        {
            var entity = new Product
            {
                ProductName = newProduct.ProductName,
                ProductType = "",
                ExpiryDate = newProduct.ExpiryDate,
                Quantity = newProduct.Quantity
            };

            _context.Products.Add(entity);
            await _context.SaveChangesAsync();
            isAddingProduct = false;
            NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
        }
        private void EditProduct(ProductsModel product)
        {
           
            selectedProduct = new ProductsModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductType = product.ProductType,
                ExpiryDate = product.ExpiryDate,
                Quantity = product.Quantity
            };

            isEditingProduct = true; 
        }

        private void CancelEditProduct()
        {
            isEditingProduct = false; 
        }

        private async Task UpdateProductInDatabase()
        {
            // Find the product in the database context
            var productEntity = await _context.Products.FindAsync(selectedProduct.ProductId);

            if (productEntity != null)
            {
               
                productEntity.ProductName = selectedProduct.ProductName;
                productEntity.ProductType = selectedProduct.ProductType;
                productEntity.ExpiryDate = selectedProduct.ExpiryDate;
                productEntity.Quantity = selectedProduct.Quantity;

              
                await _context.SaveChangesAsync();

                isEditingProduct = false;
                NavManager.NavigateTo(NavManager.Uri, forceLoad: true);


            }
        }

        private async Task DeleteProduct(ProductsModel product)
        {
            
            var productEntity = await _context.Products.FindAsync(product.ProductId);

            if (productEntity != null)
            {
                
                _context.Products.Remove(productEntity);
                await _context.SaveChangesAsync();


                NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
            }
        }
       
        private void ShowWarningDialog(string productName)
        {
            
            duplicateProducts = FilterProducts
                .Where(p => p.ProductName == productName)
                .ToList();

            isWarningDialogVisible = true;
        }
        private void CloseWarningDialog()
        {
            isWarningDialogVisible = false;
        }
    }
}
