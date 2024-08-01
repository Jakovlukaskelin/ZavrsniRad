using System;
using System.Collections.Generic;

namespace As.Zavrsni.Domain.Entites;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductType { get; set; } = null!;

    public DateOnly? ExpiryDate { get; set; }

    public int Quantity { get; set; }

    public virtual ICollection<Consumption> Consumptions { get; set; } = new List<Consumption>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
