using System;
using System.Collections.Generic;

namespace As.Zavrsni.Domain.Entites;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? ProductId { get; set; }

    public DateTime? NotificationDate { get; set; }

    public string? Message { get; set; }

    public string? Status { get; set; }

    public virtual Product? Product { get; set; }
}
