using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace As.Zavrsni.Domain.Entites
{

    [Index("Username", Name = "UQ__Users__F3DBC5729AB76B9F", IsUnique = true)]
    public partial class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("username")]
        [StringLength(50)]
        [Unicode(false)]
        public string Username { get; set; } = null!;

        [Column("password")]
        [StringLength(255)]
        [Unicode(false)]
        public string Password { get; set; } = null!;

        [Column("role_id")]
        public int? RoleId { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [ForeignKey("RoleId")]
        [InverseProperty("Users")]
        public virtual Role? Role { get; set; }
    }

}