using System;
using System.Collections.Generic;

namespace SingletonExample
{
    // 1. Singleton Sınıf Tanımı
    public sealed class CacheManager
    {
        private static CacheManager _instance;
        private static readonly object _lock = new object();
        private Dictionary<string, object> _cacheStore;

        // Private constructor: Dışarıdan 'new' yapılamaz!
        private CacheManager()
        {
            _cacheStore = new Dictionary<string, object>();
            Console.WriteLine(">> CacheManager İlk Kez Oluşturuldu (Instance Created).");
        }

        // Global Erişim Noktası
        public static CacheManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CacheManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public void Add(string key, object value)
        {
            if (!_cacheStore.ContainsKey(key))
                _cacheStore.Add(key, value);
        }

        public object Get(string key)
        {
            return _cacheStore.ContainsKey(key) ? _cacheStore[key] : null;
        }
    }

    // 2. Main Metodu (Uygulama Girişi)
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Singleton Cache Test Başladı ---");

            // HATA: Aşağıdaki satırı denersen hata alırsın çünkü constructor private!
            // CacheManager myCache = new CacheManager(); 

            // İlk çağırımda nesne oluşturulur
            CacheManager cache = CacheManager.Instance;
            cache.Add("ApiKey", "12345-ABCDE");
            cache.Add("Theme", "Dark Mode");

            Console.WriteLine("Veriler eklendi...");

            // Uygulamanın bambaşka bir yerinde tekrar Instance çağıralım
            // Yeni bir nesne oluşturulmaz, mevcut olan gelir.
            var secondRef = CacheManager.Instance;

            string apiKey = (string)secondRef.Get("ApiKey");
            string theme = (string)secondRef.Get("Theme");

            Console.WriteLine($"\nFarklı bir referanstan okunan veriler:");
            Console.WriteLine($"- API Key: {apiKey}");
            Console.WriteLine($"- Tema: {theme}");

            // Referans kontrolü (Aynı nesne mi?)
            if (Object.ReferenceEquals(cache, secondRef))
            {
                Console.WriteLine("\n[SONUÇ]: İki referans da aynı nesneyi işaret ediyor. Singleton başarıyla çalışıyor!");
            }

            Console.WriteLine("\nÇıkmak için bir tuşa basın...");
            Console.ReadKey();
        }
    }
}