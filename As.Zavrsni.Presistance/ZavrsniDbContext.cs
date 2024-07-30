using System;
using System.Collections.Generic;
using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace As.Zavrsni.Presistance;

public partial class ZavrsniDbContext : DbContext, IZavrsniDbContext
{
    public ZavrsniDbContext()
    {
    }

    public ZavrsniDbContext(DbContextOptions<ZavrsniDbContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Consumption> Consumptions { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public async Task<List<T>> SqlQuery<T>(System.FormattableString query)
    {
        return await this.Database.SqlQuery<T>(query).ToListAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await this.SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Consumption>(entity =>
        {
            entity.HasKey(e => e.ConsumptionId).HasName("PK__Consumpt__FBC0AE6B4ED27D0F");

            entity.HasOne(d => d.Product).WithMany(p => p.Consumptions).HasConstraintName("FK__Consumpti__produ__70DDC3D8");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__B59ACC493774BAEB");

            entity.HasOne(d => d.Product).WithMany(p => p.Inventories).HasConstraintName("FK__Inventory__produ__6477ECF3");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842FAFE977B6");

            entity.Property(e => e.NotificationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("unread");

            entity.HasOne(d => d.Product).WithMany(p => p.Notifications).HasConstraintName("FK__Notificat__produ__6E01572D");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__46596229EE3EA5DE");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders).HasConstraintName("FK__Orders__product___693CA210");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasConstraintName("FK__Orders__user_id__68487DD7");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__47027DF5FCC885EC");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CCFE52BB54");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FEA474FB6");

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("FK__Users__role_id__5FB337D6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
