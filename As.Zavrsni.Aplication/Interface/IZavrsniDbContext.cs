
using As.Zavrsni.Domain.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Interface
{
    public interface IZavrsniDbContext
    {
        DbSet<Consumption> Consumptions { get; set; }

        DbSet<Inventory> Inventories { get; set; }

        DbSet<Notification> Notifications { get; set; }

        DbSet<Order> Orders { get; set; }

        DbSet<Product> Products { get; set; }

         DbSet<Role> Roles { get; set; }

        DbSet<User> Users { get; set; }
        Task<List<T>> SqlQuery<T>(System.FormattableString query);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
