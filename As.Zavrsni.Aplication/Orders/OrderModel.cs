using As.Zavrsni.Aplication.Interface.Mapping;
using As.Zavrsni.Aplication.Products.Model;
using As.Zavrsni.Domain.Entites;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Orders
{
    public class OrderModel : IHaveCustomMapping
    {
        public int OrderId { get; set; }

        public int? UserId { get; set; }

        public int? ProductId { get; set; }

        public DateTime? OrderDate { get; set; }

        public int Quantity { get; set; }

        public string UserName { get; set; }

        public string ProductName { get; set; }
        public List<ProductsModel> Products { get; set; } = new List<ProductsModel>();

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<Product, OrderModel>();

            configuration.CreateMap<Order, OrderModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                 .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                 .ForMember(dest => dest.Products, opt => opt.MapFrom(src =>
                     src.Product != null ? new List<ProductsModel> { new ProductsModel {
                        ProductId = src.Product.ProductId,
                        ProductName = src.Product.ProductName,
                        ProductType = src.Product.ProductType,
                        ExpiryDate = src.Product.ExpiryDate,
                        Quantity = src.Product.Quantity } }
                     : new List<ProductsModel>()));
        }
    }
}
