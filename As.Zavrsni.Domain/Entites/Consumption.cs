using System;
using System.Collections.Generic;

namespace As.Zavrsni.Domain.Entites;

public partial class Consumption
{
    public int ConsumptionId { get; set; }

    public int? ProductId { get; set; }

    public DateOnly? ConsumptionDate { get; set; }

    public int Quantity { get; set; }

    public string Type { get; set; } = null!;

    public virtual Product? Product { get; set; }
}
