using System;
using System.Collections.Generic;

namespace As.Zavrsni.Domain.Entites;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int? ProductId { get; set; }

    public int CurrentQuantity { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public virtual Product? Product { get; set; }
}
