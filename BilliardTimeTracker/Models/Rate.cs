using System;
using System.Collections.Generic;

namespace BilliardTimeTracker.Models;

public partial class Rate
{
    public int RateId { get; set; }

    public string RateName { get; set; } = null!;

    public decimal RatePerHour { get; set; }

    public bool? Active { get; set; }

    public virtual ICollection<TableRate> TableRates { get; set; } = new List<TableRate>();
}
