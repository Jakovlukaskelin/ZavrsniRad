using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace As.Zavrsni.Domain.Entites
{
    [Table("Consumption")]
    public partial class Consumption
    {
        [Key]
        [Column("consumption_id")]
        public int ConsumptionId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Column("consumption_date")]
        public DateOnly? ConsumptionDate { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("Consumptions")]
        public virtual Product? Product { get; set; }
    }
}
