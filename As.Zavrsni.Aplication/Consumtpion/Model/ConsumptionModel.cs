using As.Zavrsni.Aplication.Interface.Mapping;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using As.Zavrsni.Domain.Entites;
using System.Threading.Tasks;
using As.Zavrsni.Aplication.Products.Model;
using Microsoft.AspNetCore.SignalR;

namespace As.Zavrsni.Aplication.Consumtpion.Model
{
    public class ConsumptionModel : IHaveCustomMapping
    {
        public int ConsumptionId { get; set; }

        public int? ProductId { get; set; }

        public string ProductName { get; set; }

        public DateOnly? ConsumptionDate { get; set; }

        public int Quantity { get; set; }

        

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<Consumption, ConsumptionModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom( src => src.Product.ProductName));

            configuration.CreateMap<Product, ConsumptionModel>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.ConsumptionDate, opt => opt.MapFrom(src => src.ExpiryDate))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            
        }
    }
}
