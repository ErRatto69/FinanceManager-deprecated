using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Services
{
    public class GroceryService
    {
        private readonly HomeFinanceContext _context;
        private readonly UserSessionService _session; // <--- INIETTIAMO LA SESSIONE

        public GroceryService(HomeFinanceContext context, UserSessionService session)
        {
            _context = context;
            _session = session;
        }

        public async Task<List<GroceryListItem>> GetActiveListAsync()
        {
            return await _context.GroceryListItems
                .Include(i => i.Product)
                .Include(i => i.AddedByUser) // <--- CARICHIAMO L'UTENTE
                .OrderBy(i => i.IsChecked)
                .ThenBy(i => i.Product.Name)
                .ToListAsync();
        }

        public async Task AddItemAsync(string productName)
        {
            var product = await _context.GroceryProducts
                .FirstOrDefaultAsync(p => p.Name == productName);

            if (product == null)
            {
                product = new GroceryProduct { Name = productName, DefaultPrice = 0 };
                _context.GroceryProducts.Add(product);
                await _context.SaveChangesAsync();
            }

            var item = new GroceryListItem
            {
                ProductId = product.Id,
                Quantity = 1,
                Unit = "pz",
                IsChecked = false,
                // SALVIAMO L'ID DELL'UTENTE LOGGATO
                AddedByUserId = _session.CurrentUser?.Id ?? 1 
            };

            _context.GroceryListItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task ToggleItemAsync(GroceryListItem item)
        {
            item.IsChecked = !(item.IsChecked ?? false);
            _context.GroceryListItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int itemId)
        {
            var item = await _context.GroceryListItems.FindAsync(itemId);
            if (item != null)
            {
                _context.GroceryListItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task CheckoutAsync(decimal actualTotalSpent)
        {
            var transaction = new Transaction
            {
                Title = "Spesa Supermercato",
                Amount = actualTotalSpent,
                Currency = "CHF",
                ExchangeRate = 1,
                DueDate = DateOnly.FromDateTime(DateTime.Now),
                IsPaid = true,
                PaidDate = DateOnly.FromDateTime(DateTime.Now),
                CreatedByUserId = _session.CurrentUser?.Id ?? 1, // <--- ANCHE QUI
                CategoryId = 2 
            };

            _context.Transactions.Add(transaction);
            
            var allItems = await _context.GroceryListItems.ToListAsync();
            _context.GroceryListItems.RemoveRange(allItems);

            await _context.SaveChangesAsync();
        }

        public async Task<List<GroceryProduct>> GetAllProductsAsync()
        {
            return await _context.GroceryProducts.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task UpdateProductPriceAsync(int productId, decimal newPrice)
        {
            var product = await _context.GroceryProducts.FindAsync(productId);
            if (product != null)
            {
                product.DefaultPrice = newPrice;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteProductFromCatalogAsync(string productName)
        {
            var product = await _context.GroceryProducts.FirstOrDefaultAsync(p => p.Name == productName);
            if (product != null)
            {
                var listItems = await _context.GroceryListItems.Where(i => i.ProductId == product.Id).ToListAsync();
                _context.GroceryListItems.RemoveRange(listItems);
                _context.GroceryProducts.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateItemDetailsAsync(int itemId, decimal quantity, string unit)
        {
            var item = await _context.GroceryListItems.FindAsync(itemId);
            if (item != null)
            {
                item.Quantity = quantity;
                item.Unit = unit;
                await _context.SaveChangesAsync();
            }
        }
    }
}