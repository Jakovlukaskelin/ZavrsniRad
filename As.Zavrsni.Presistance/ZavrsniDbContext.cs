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
            entity.HasKey(e => e.ConsumptionId).HasName("PK__Consumpt__FBC0AE6B33206568");

            entity.ToTable("Consumption");

            entity.Property(e => e.ConsumptionId).HasColumnName("consumption_id");
            entity.Property(e => e.ConsumptionDate).HasColumnName("consumption_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.Product).WithMany(p => p.Consumptions)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Consumpti__produ__74AE54BC");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__B59ACC4944896931");

            entity.ToTable("Inventory");

            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.CurrentQuantity).HasColumnName("current_quantity");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Inventory__produ__6477ECF3");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842F4E16A067");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.Message)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("message");
            entity.Property(e => e.NotificationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("notification_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("unread")
                .HasColumnName("status");

            entity.HasOne(d => d.Product).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Notificat__produ__6E01572D");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__465962292E4450D6");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Orders__product___693CA210");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__user_id__68487DD7");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__47027DF5319BC1F4");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("product_name");
            entity.Property(e => e.ProductType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("product_type");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CCAEAC8F62");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FD5C5AB3B");

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC572EBB1A57E").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__role_id__5FB337D6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
