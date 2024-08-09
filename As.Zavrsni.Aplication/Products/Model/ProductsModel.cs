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

            configuration.CreateMap<Consumption, ProductsModel>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.Product.ProductType))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.Product.ExpiryDate))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
               
        }
    }
}
