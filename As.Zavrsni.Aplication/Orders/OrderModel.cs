using As.Zavrsni.Aplication.Interface.Mapping;
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


        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<Order, OrderModel>();
        }
    }
}
