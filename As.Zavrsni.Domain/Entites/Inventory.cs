using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace As.Zavrsni.Domain.Entites
{
    [Table("Inventory")]
    public partial class Inventory
    {
        [Key]
        [Column("inventory_id")]
        public int InventoryId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Column("current_quantity")]
        public int CurrentQuantity { get; set; }

        [Column("expiry_date")]
        public DateOnly? ExpiryDate { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("Inventories")]
        public virtual Product? Product { get; set; }
    }
}