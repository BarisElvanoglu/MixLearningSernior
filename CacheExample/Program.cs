using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory; // In-Memory için

namespace CacheComparisonDemo
{
    public class CacheService
    {
        // 1. In-Memory Cache (L1 - En hızlı)
        private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        // 2. Redis Simülasyonu (L2 - Dağıtık)
        // Gerçekte burada StackExchange.Redis kütüphanesi kullanılır.
        private readonly Dictionary<string, string> _redisDb = new();

        public async Task<string> GetUserDataAsync(int userId)
        {
            string cacheKey = $"user_{userId}";

            // --- ADIM 1: IN-MEMORY CACHE KONTROLÜ ---
            if (_memoryCache.TryGetValue(cacheKey, out string cachedData))
            {
                Console.WriteLine("[L1 - IN-MEMORY] Veri RAM'den anında getirildi.");
                return cachedData;
            }

            // --- ADIM 2: REDIS (DISTRIBUTED) CACHE KONTROLÜ ---
            if (_redisDb.TryGetValue(cacheKey, out string redisData))
            {
                Console.WriteLine("[L2 - REDIS] Veri Redis'ten (Ağ üzerinden) getirildi.");

                // Redis'ten bulduğumuz veriyi bir sonraki sefer için In-Memory'ye de atıyoruz (L1'i doldur)
                _memoryCache.Set(cacheKey, redisData, TimeSpan.FromMinutes(1));
                return redisData;
            }

            // --- ADIM 3: VERİTABANI (ASIL KAYNAK) ---
            Console.WriteLine("[DB] Veri veritabanından ağır ağır çekiliyor...");
            await Task.Delay(1000); // Veritabanı gecikmesi simülasyonu
            string dbData = $"User_Data_For_{userId}";

            // Bulduğumuz veriyi hem Redis'e hem In-Memory'ye yazıyoruz
            _redisDb[cacheKey] = dbData;
            _memoryCache.Set(cacheKey, dbData, TimeSpan.FromMinutes(1));

            return dbData;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var cacheService = new CacheService();
            int userId = 42;

            Console.WriteLine("--- 1. ÇALIŞTIRMA (Cache Boş) ---");
            var data1 = await cacheService.GetUserDataAsync(userId);
            Console.WriteLine($"Sonuç: {data1}\n");

            Console.WriteLine("--- 2. ÇALIŞTIRMA (In-Memory'den gelecek) ---");
            var data2 = await cacheService.GetUserDataAsync(userId);
            Console.WriteLine($"Sonuç: {data2}\n");

            // Senaryo: Uygulama restart attı, In-Memory silindi ama Redis yaşıyor!
            Console.WriteLine("--- 3. ÇALIŞTIRMA (In-Memory manuel siliniyor, Redis'ten bekleniyor) ---");
            // (Burada In-Memory'nin temizlendiğini varsayalım)
            var serviceAfterRestart = new CacheService(); // Yeni instance = Boş In-Memory

            // Not: Gerçekte Redis dış bir sunucu olduğu için veriler onda hala durur.
            // Bu simülasyonda Redis verisini korumak için aynı service üzerinden devam edelim 
            // ama L1'in olmadığını hayal edelim.

            var data3 = await cacheService.GetUserDataAsync(userId);
            Console.WriteLine($"Sonuç: {data3}\n");
        }
    }
}