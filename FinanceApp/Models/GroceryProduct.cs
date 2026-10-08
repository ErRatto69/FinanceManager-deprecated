using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class GroceryProduct
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal? DefaultPrice { get; set; }

    public virtual ICollection<GroceryListItem> GroceryListItems { get; set; } = new List<GroceryListItem>();
}
