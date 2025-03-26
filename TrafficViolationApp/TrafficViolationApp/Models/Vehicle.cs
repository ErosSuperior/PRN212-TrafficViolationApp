using System;
using System.Collections.Generic;

namespace TrafficViolationApp.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public string PlateNumber { get; set; } = null!;

    public int OwnerId { get; set; }

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public int? ManufactureYear { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
