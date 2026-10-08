using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceApp.Models
{
    public class GroceryListItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public GroceryProduct? Product { get; set; }

        public bool? IsChecked { get; set; }

        // Quantità (es. 1.5)
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Quantity { get; set; } = 1; 

        // Unità (es. kg, pz)
        [MaxLength(10)]
        public string Unit { get; set; } = "pz"; 

        // Chiave esterna utente
        public int AddedByUserId { get; set; }

        // --- QUESTA ERA LA PARTE MANCANTE CHE CAUSAVA L'ERRORE ---
        [ForeignKey("AddedByUserId")]
        public virtual AppUser? AddedByUser { get; set; }
    }
}