using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventualConsistencyDemo
{
    // 1. Veri Modelimiz
    public record Product(int Id, string Name);

    // 2. Sistem Sınıfı (Verinin nasıl dağıtıldığını simüle eder)
    public class ProductService
    {
        // Mutlak tutarlı olan ana kaynak (Örn: SQL Server)
        private readonly Dictionary<int, Product> _writeDatabase = new();

        // Gecikmeli tutarlı olan hızlı kaynak (Örn: Redis veya ElasticSearch)
        private readonly Dictionary<int, Product> _readDatabase = new();

        // VERİ YAZMA (COMMAND)
        public async Task SaveProductAsync(Product product)
        {
            Console.WriteLine($"[1. ADIM] Veri ana veritabanına yazılıyor: {product.Name}");
            _writeDatabase[product.Id] = product;

            // Arka planda okuma veritabanını güncellemeye başla (BEKLEME YAPMA!)
            _ = UpdateReadDatabaseAsync(product);

            Console.WriteLine("[2. ADIM] Yazma işlemi kullanıcıya 'BAŞARILI' döndü.");
        }

        // Arka planda çalışan senkronizasyon (Gecikmeyi yaratan kısım burası)
        private async Task UpdateReadDatabaseAsync(Product product)
        {
            // Sistemsel gecikme simülasyonu (Ağ trafiği, kuyruk bekleme vb.)
            await Task.Delay(3000);

            _readDatabase[product.Id] = product;
            Console.WriteLine($"\n[BİLGİ] Okuma veritabanı GÜNCELLENDİ: {product.Name} artık sorgulanabilir.");
        }

        // VERİ OKUMA (QUERY)
        public Product GetProductFromReadDb(int id)
        {
            _readDatabase.TryGetValue(id, out var product);
            return product;
        }
    }

    // 3. ANA PROGRAM
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("--- Eventual Consistency Simülasyonu Başladı ---\n");

            var service = new ProductService();
            int productId = 1;

            // Yeni bir ürün kaydediyoruz
            await service.SaveProductAsync(new Product(productId, "Akıllı Telefon"));

            // Kayıt hemen biter bitmez veriyi OKUMAYA çalışıyoruz
            Console.WriteLine("\n[SORGU] Kayıttan hemen sonra veri okunuyor...");
            var result1 = service.GetProductFromReadDb(productId);

            if (result1 == null)
                Console.WriteLine("-> SONUÇ: Veri henüz hazır değil (Tutarsızlık Anı!)");

            // Biraz bekliyoruz (Sistemin tutarlı hale gelmesi için süre tanıyoruz)
            Console.WriteLine("\n[SİSTEM] 4 saniye bekleniyor...");
            await Task.Delay(4000);

            // Tekrar okuyoruz
            Console.WriteLine("[SORGU] Tekrar okunuyor...");
            var result2 = service.GetProductFromReadDb(productId);

            if (result2 != null)
                Console.WriteLine($"-> SONUÇ: Veri geldi: {result2.Name} (Sistem artık TUTARLI)");

            Console.WriteLine("\nProgram sonlandı.");
        }
    }
}