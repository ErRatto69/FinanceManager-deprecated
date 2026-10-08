using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class AppUser
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = "Basic";

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<GroceryListItem> GroceryListItems { get; set; } = new List<GroceryListItem>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
