using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace As.Zavrsni.Domain.Entites
{
    public partial class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Column("order_date", TypeName = "datetime")]
        public DateTime? OrderDate { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("Orders")]
        public virtual Product? Product { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Orders")]
        public virtual User? User { get; set; }
    }

}