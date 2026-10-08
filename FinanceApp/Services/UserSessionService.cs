using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Services
{
    public class UserSessionService
    {
        private readonly HomeFinanceContext _context;
        
        public AppUser? CurrentUser { get; private set; }
        public event Action? OnChange;

        public UserSessionService(HomeFinanceContext context)
        {
            _context = context;
        }

        // --- NUOVO: Login con verifica Password ---
        public async Task<bool> LoginWithCredentialsAsync(string username, string password)
        {
            // Cerca l'utente nel DB (Case insensitive)
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            // Controllo Password (in un'app vera useremmo hash, qui testo semplice per ora)
            if (user != null && user.PasswordHash == password)
            {
                CurrentUser = user;
                NotifyStateChanged();
                return true; // Login OK
            }

            return false; // Login Fallito
        }

        public void Logout()
        {
            CurrentUser = null;
            NotifyStateChanged();
        }

        // PERMESSI
        public bool IsAdmin => CurrentUser?.Role == "Admin";
        public bool CanManageFinances => CurrentUser?.Role == "Admin" || CurrentUser?.Role == "Standard";
        public bool IsLoggedIn => CurrentUser != null;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}