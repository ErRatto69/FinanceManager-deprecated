using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Services
{
    public class DashboardService
    {
        private readonly HomeFinanceContext _context;
        private readonly UserSessionService _session; // <--- INIEZIONE SESSIONE

        public DashboardService(HomeFinanceContext context, UserSessionService session)
        {
            _context = context;
            _session = session;
        }

        public async Task<List<Transaction>> GetMonthlySpentAsync() => await _context.Transactions.Where(t => t.IsPaid && t.PaidDate >= new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1)).ToListAsync();
        
        public async Task<decimal> GetMonthlySpentTotalAsync()
        {
            var txs = await GetMonthlySpentAsync();
            return txs.Sum(t => t.Amount * (t.ExchangeRate ?? 1));
        }

        public async Task<List<Transaction>> GetUnpaidBillsAsync()
        {
            return await _context.Transactions
                .Include(t => t.CreatedByUser) // <--- CARICA UTENTE
                .Where(t => !t.IsPaid)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<List<Subscription>> GetActiveSubscriptionsAsync() => await _context.Subscriptions.Include(s => s.Category).Where(s => s.IsActive == true).OrderBy(s => s.RenewalDay).ToListAsync();

        public async Task<List<Category>> GetCategoriesAsync() => await _context.Categories.ToListAsync();

        public async Task AddTransactionAsync(Transaction trans)
        {
            if (trans.CategoryId == 0) trans.CategoryId = 1; 
            if (trans.Currency == "EUR") trans.Amount = trans.Amount * 0.95m;
            
            // SALVA UTENTE LOGGATO
            trans.CreatedByUserId = _session.CurrentUser?.Id ?? 1;

            _context.Transactions.Add(trans);
            await _context.SaveChangesAsync();
        }

        public async Task AddSubscriptionAsync(Subscription sub)
        {
            if (sub.CategoryId == 0) sub.CategoryId = 1; 
            sub.IsActive = true; 
            if (sub.Currency == "EUR") sub.Amount = sub.Amount * 0.95m;
            _context.Subscriptions.Add(sub);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsPaidAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null) { transaction.IsPaid = true; transaction.PaidDate = DateOnly.FromDateTime(DateTime.Now); await _context.SaveChangesAsync(); }
        }

        public async Task<List<Transaction>> GetRecentPaidTransactionsAsync()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateOnly(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            return await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.CreatedByUser) // <--- CARICA UTENTE
                .Where(t => t.IsPaid && t.PaidDate >= startOfMonth && t.PaidDate <= endOfMonth) 
                .OrderByDescending(t => t.PaidDate)
                .ToListAsync();
        }

        // CRUD
        public async Task<Transaction?> GetTransactionByIdAsync(int id) => await _context.Transactions.FindAsync(id);
        public async Task<Subscription?> GetSubscriptionByIdAsync(int id) => await _context.Subscriptions.FindAsync(id);
        public async Task UpdateTransactionAsync(Transaction t) { _context.Transactions.Update(t); await _context.SaveChangesAsync(); }
        public async Task UpdateSubscriptionAsync(Subscription s) { _context.Subscriptions.Update(s); await _context.SaveChangesAsync(); }
        public async Task DeleteTransactionAsync(int id) { var t = await _context.Transactions.FindAsync(id); if (t != null) { _context.Transactions.Remove(t); await _context.SaveChangesAsync(); } }
        public async Task DeleteSubscriptionAsync(int id) { var s = await _context.Subscriptions.FindAsync(id); if (s != null) { _context.Subscriptions.Remove(s); await _context.SaveChangesAsync(); } }

        // STATS
        public async Task<List<CategoryStat>> GetMonthlyStatsAsync(int month, int year)
        {
            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var transactions = await _context.Transactions.Where(t => t.IsPaid && t.PaidDate >= startDate && t.PaidDate <= endDate).ToListAsync();

            var stats = transactions.GroupBy(t => t.CategoryId).Select(g => new CategoryStat { CategoryId = g.Key, TotalAmount = g.Sum(t => t.Amount * (t.ExchangeRate ?? 1)), TransactionCount = g.Count() }).OrderByDescending(s => s.TotalAmount).ToList();

            string[] colors = { "#0d6efd", "#198754", "#dc3545", "#ffc107", "#0dcaf0", "#6610f2", "#d63384", "#fd7e14", "#20c997", "#6c757d" };
            int colorIndex = 0;

            foreach (var stat in stats)
            {
                var cat = await _context.Categories.FindAsync(stat.CategoryId);
                if (cat != null) { stat.CategoryName = cat.Name; stat.Icon = cat.Icon; stat.Color = colors[colorIndex % colors.Length]; colorIndex++; }
            }
            return stats;
        }

        public class CategoryStat { public int CategoryId { get; set; } public string CategoryName { get; set; } = "Altro"; public decimal TotalAmount { get; set; } public int TransactionCount { get; set; } public string Color { get; set; } = "#6c757d"; public string Icon { get; set; } = "bi-tag"; }
    }
}