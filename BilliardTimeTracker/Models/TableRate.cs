using System;
using System.Collections.Generic;

namespace BilliardTimeTracker.Models;

public partial class TableRate
{
    public int TableRateId { get; set; }

    public int TableId { get; set; }

    public int RateId { get; set; }

    public virtual Rate Rate { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
