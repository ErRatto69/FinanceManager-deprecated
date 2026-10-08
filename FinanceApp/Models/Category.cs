using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Icon { get; set; }

    public string? ColorHex { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
