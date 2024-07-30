using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace As.Zavrsni.Domain.Entites
{
    public partial class Notification
    {
        [Key]
        [Column("notification_id")]
        public int NotificationId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Column("notification_date", TypeName = "datetime")]
        public DateTime? NotificationDate { get; set; }

        [Column("message")]
        [StringLength(255)]
        [Unicode(false)]
        public string? Message { get; set; }

        [Column("status")]
        [StringLength(50)]
        [Unicode(false)]
        public string? Status { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("Notifications")]
        public virtual Product? Product { get; set; }
    }
}
