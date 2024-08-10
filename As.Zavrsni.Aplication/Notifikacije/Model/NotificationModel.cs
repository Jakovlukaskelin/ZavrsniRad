using As.Zavrsni.Aplication.Interface.Mapping;
using As.Zavrsni.Domain.Entites;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Notifikacije.Model
{
    public class NotificationModel : IHaveCustomMapping
    {
        public int NotificationId { get; set; }

        public int? ProductId { get; set; }

        public DateTime? NotificationDate { get; set; }

        public string Message { get; set; }

        public bool Status { get; set; } = false;
        public string ProductName { get; set; } = null!;

        public string ProductType { get; set; } = null!;

        public DateOnly? ExpiryDate { get; set; }

        public int Quantity { get; set; }

        public void CreateMappings(Profile configuration)
        {

            configuration.CreateMap<Notification, NotificationModel>()
     .ForMember(d => d.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : null))
     .ForMember(d => d.ProductType, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductType : null))
     .ForMember(d => d.ExpiryDate, opt => opt.MapFrom(src => src.Product != null ? src.Product.ExpiryDate : null))
     .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Product != null ? src.Product.Quantity : 0))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToLower() == "true"));

            configuration.CreateMap<Product, NotificationModel>();


        }
    }
}
