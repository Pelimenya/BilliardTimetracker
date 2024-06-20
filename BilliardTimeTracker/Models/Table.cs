using System;
using System.Collections.Generic;

namespace BilliardTimeTracker.Models;

public partial class Table
{
    public int TableId { get; set; }

    public string TableName { get; set; } = null!;

    public string? Location { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<TableRate> TableRates { get; set; } = new List<TableRate>();
}
