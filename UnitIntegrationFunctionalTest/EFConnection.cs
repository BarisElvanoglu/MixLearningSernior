using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

// --- 1. Veritabanı Bağlamı (AppDbContext) ---
namespace UnitIntegrationFunctionalTest
{


    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; } // Order tablosu
    }

    // --- 2. Veri Modeli ---
    public class Order
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
    }

    // --- 3. Repository Arayüzü ve İmplementasyonu ---
    public interface IRepository
    {
        Task<bool> Save(Order o);
    }

    public class SqlRepository : IRepository
    {
        private readonly AppDbContext _context;
        public SqlRepository(AppDbContext context) => _context = context;

        public async Task<bool> Save(Order o)
        {
            _context.Orders.Add(o);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    // --- 4. OrderService (İş Mantığı) ---
    public class OrderService
    {
        private readonly IRepository _repo;
        public OrderService(IRepository repo) => _repo = repo;

        public async Task<bool> ProcessOrder(Order order)
        {
            // Örnek bir mantık: Fiyat 100'den büyükse %10 indirim
            if (order.Price > 100) order.Price *= 0.9m;
            return await _repo.Save(order);
        }
    }
}