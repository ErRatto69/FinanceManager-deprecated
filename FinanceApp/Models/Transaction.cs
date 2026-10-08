using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public decimal? ExchangeRate { get; set; }

    public decimal? AmountInChf { get; set; }

    public DateOnly DueDate { get; set; }

    public bool IsPaid { get; set; }

    public DateOnly? PaidDate { get; set; }

    public int CategoryId { get; set; }

    public int CreatedByUserId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual AppUser CreatedByUser { get; set; } = null!;
}
