﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace As.Zavrsni.Domain.Entites
{
    public partial class Role
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("role_name")]
        [StringLength(50)]
        [Unicode(false)]
        public string RoleName { get; set; } = null!;

        [InverseProperty("Role")]
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}