using System;
using System.Collections.Generic;

namespace BilliardTimeTracker.Models;

public partial class Session
{
    public int SessionId { get; set; }

    public int TableId { get; set; }

    public int UserId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal? Cost { get; set; }

    public virtual Table Table { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
