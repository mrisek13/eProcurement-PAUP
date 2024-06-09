using System.Data.Entity;

namespace eProcurement_PAUP.Models
{
    public class BazaDbContext : DbContext
    {
        // Dodan konstruktor koji prima ime connection stringa
        public BazaDbContext() : base("MyConnectionString")
        {
        }

        // DbSet-ovi za entitete vaše baze podataka
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; } // Ispravljen naziv DbSet-a

        // DbSet za OrderViewModel, ako je potrebno
        public DbSet<OrderViewModel> OrderViewModels { get; set; }
    }
}
