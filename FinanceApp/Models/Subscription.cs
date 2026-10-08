using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class Subscription
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public int RenewalDay { get; set; }

    public int CategoryId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Category Category { get; set; } = null!;
}
