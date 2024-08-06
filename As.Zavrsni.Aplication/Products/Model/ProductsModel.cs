using As.Zavrsni.Aplication.Interface.Mapping;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using As.Zavrsni.Domain.Entites;

namespace As.Zavrsni.Aplication.Products.Model
{
    public class ProductsModel : IHaveCustomMapping
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductType { get; set; } 

        public DateOnly? ExpiryDate { get; set; }

        public int Quantity { get; set; }

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<Product, ProductsModel>();
        }
    }
}
