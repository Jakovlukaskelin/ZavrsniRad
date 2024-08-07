using As.Zavrsni.Aplication.Interface.Mapping;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using As.Zavrsni.Domain.Entites;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Consumtpion.Model
{
    public class ConsumptionModel : IHaveCustomMapping
    {
        public int ConsumptionId { get; set; }

        public int? ProductId { get; set; }

        public DateOnly? ConsumptionDate { get; set; }

        public int Quantity { get; set; }

        public string Type { get; set; } = null!;

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<Consumption, ConsumptionModel>();
        }
    }
}
