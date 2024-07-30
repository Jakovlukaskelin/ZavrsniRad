using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace As.Zavrsni.Domain.Entites
{
    public partial class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("product_name")]
        [StringLength(100)]
        [Unicode(false)]
        public string ProductName { get; set; } = null!;

        [Column("product_type")]
        [StringLength(50)]
        [Unicode(false)]
        public string ProductType { get; set; } = null!;

        [Column("expiry_date")]
        public DateOnly? ExpiryDate { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<Consumption> Consumptions { get; set; } = new List<Consumption>();

        [InverseProperty("Product")]
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        [InverseProperty("Product")]
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        [InverseProperty("Product")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
