using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== WeakReference ile Smart Cache - Düzeltilmiş Demo ===\n");

        var cache = new WeakReferenceCache<string, CachedObject>();
        cache.ItemEvicted += (key) => Console.WriteLine($"  ⚠️  Cache Event: '{key}' bellekten temizlendi!");

        // 1 & 2 & 3) Nesneleri ayrı bir metodda oluşturuyoruz ki scope kapansın
        CreateAndCacheObjects(cache);


        // Bu noktada obj1, obj2, obj3 referansları stack'ten temizlendi.

        Console.WriteLine("\n4) GC zorlanıyor...");
        // GC'yi tam temizlik yapması için zorluyoruz
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Console.WriteLine($"\nCache Count (GC sonrası): {cache.Count}");

        Console.WriteLine("\n5) Cache kontrol ediliyor:");
        if (!cache.TryGet("customer_1", out var retrieved1))
        {
            Console.WriteLine("✗ 'customer_1' artık cache'de yok (Beklenen durum).");
        }
        else
        {
            Console.WriteLine($"✓ '{retrieved1.Id}' hala bulundu (Beklenmeyen durum - JIT hala tutuyor olabilir).");
        }

        // Yeni nesne ekle
        var obj4 = new CachedObject("Müşteri-004", 256);
        cache.Add("customer_4", obj4);

        if (cache.TryGet("customer_4", out var obj4Retrieved))
            Console.WriteLine($"✓ '{obj4Retrieved.Id}' hala cache'de (Güçlü referans mevcut).");

        Console.WriteLine("\nDemo tamamlandı. Bir tuşa basın...");
        Console.ReadKey();
    }

    // Bu metod bittiğinde içindeki yerel referanslar (obj1, obj2, obj3) ölür.d
    static void CreateAndCacheObjects(WeakReferenceCache<string, CachedObject> cache)
    {
        Console.WriteLine("1) Cache'e nesneler ekleniyor...");
        var obj1 = new CachedObject("Müşteri-001", 1024);
        var obj2 = new CachedObject("Müşteri-002", 2048);
        var obj3 = new CachedObject("Müşteri-003", 512);

        cache.Add("customer_1", obj1);
        cache.Add("customer_2", obj2);
        cache.Add("customer_3", obj3);

        Console.WriteLine("2) İlk aşamada veri çekme denemesi:");
        if (cache.TryGet("customer_1", out var r))
            Console.WriteLine($"✓ '{r.Id}' hala stack'te canlı.");
    }
}

public class CachedObject
{
    public string Id { get; set; }
    public int SizeKB { get; set; }
    private byte[] _data;

    public CachedObject(string id, int sizeKB)
    {
        Id = id;
        SizeKB = sizeKB;
        _data = new byte[sizeKB * 1024];
        Console.WriteLine($"  [Oluşturuldu] {id} ({sizeKB} KB)");
    }

    ~CachedObject()
    {
        // Bu mesajı gördüğünde GC'nin çalıştığından emin olabilirsin
        Console.WriteLine($"  💀 [Finalizer] '{Id}' bellekten silindi!");
    }
}

public class WeakReferenceCache<TKey, TValue> where TValue : class
{
    private readonly Dictionary<TKey, WeakReference<TValue>> _cache = new();
    public event Action<TKey> ItemEvicted;

    public int Count
    {
        get
        {
            CleanupDeadReferences();
            return _cache.Count;
        }
    }

    public void Add(TKey key, TValue value)
    {
        _cache[key] = new WeakReference<TValue>(value);
        Console.WriteLine($"  [Cache] '{key}' eklendi.");
    }

    public bool TryGet(TKey key, out TValue value)
    {
        value = null;
        if (!_cache.TryGetValue(key, out var weakRef)) return false;

        if (weakRef.TryGetTarget(out value)) return true;

        _cache.Remove(key);
        ItemEvicted?.Invoke(key);
        return false;
    }

    private void CleanupDeadReferences()
    {
        var deadKeys = new List<TKey>();
        foreach (var kvp in _cache)
        {
            if (!kvp.Value.TryGetTarget(out _))
                deadKeys.Add(kvp.Key);
        }

        foreach (var key in deadKeys)
        {
            _cache.Remove(key);
            ItemEvicted?.Invoke(key);
        }
    }
}