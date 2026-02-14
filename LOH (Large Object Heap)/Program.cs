using System;
using System.Diagnostics;
using System.Collections.Generic;

Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║         Large Object Heap (LOH) Performans Analizi                ║");
Console.WriteLine("║                          (Türkçe Açıklamalar)                     ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝\n");

// ===== 1. LOH NEDİR =====
Console.WriteLine("📌 LOH (Large Object Heap) Tanımı:");
Console.WriteLine("  • 85 KB'tan büyük nesneler LOH'da saklanır (.NET Framework'de 85KB sınırı)");
Console.WriteLine("  • LOH, küçük nesnelerden ayrı bir heap'dir");
Console.WriteLine("  • Tek yönlü kompaksiyon (defragmentation) yapılmaz (kaynaklar çoğunlukla)");
Console.WriteLine("  • Gen0 ve Gen1'e tabi değildir, doğrudan Gen2'ye gider\n");

// ===== 2. PERFORMANS SORUNLARI =====
Console.WriteLine("⚠️  Performans Sorunları:");
Console.WriteLine("  1. Belleksel Parçalanma (Fragmentation):");
Console.WriteLine("     - LOH'da nesneler kompakt olmayan şekilde saklanır");
Console.WriteLine("     - Silinen nesneler arasında boş alanlar kalmaz (internalize edilmez)");
Console.WriteLine("     - Bellek verimsizliğine yol açar\n");
Console.WriteLine("  2. GC Taraması Yavaş:");
Console.WriteLine("     - LOH'daki tüm nesneler Gen2 ile birlikte taranır");
Console.WriteLine("     - Büyük Gen2 toplamaları performansı etkiler\n");
Console.WriteLine("  3. Full GC Sıklığı Artması:");
Console.WriteLine("     - LOH dolunca Gen2 toplaması tetiklenir");
Console.WriteLine("     - Uygulamanın tamamı durdurulur (stop-the-world)\n");

// ===== 3. PRATIK ÖRNEK: LOH PROBLEMATI =====
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

DemoSmallObjectsVsLargeObjects();

Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

DemoLOHFragmentation();

Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

DemoBestPractices();

Console.WriteLine("\n✅ Örnek tamamlandı.\n");

// ===== FONKSIYONLAR =====

void DemoSmallObjectsVsLargeObjects()
{
    Console.WriteLine("📊 DEMO 1: Küçük vs Büyük Nesneler (GC Davranışı)\n");

    // Küçük nesneler
    Console.WriteLine("1️⃣  KÜÇÜK NESNELER (10 KB):");
    Console.WriteLine($"   Bellek Öncesi: {GC.GetTotalMemory(false) / 1024_000} MB");

    var stopwatch = Stopwatch.StartNew();
    for (int i = 0; i < 100_000; i++)
    {
        byte[] small = new byte[10_000]; // 10 KB - Gen0'da başlar
    }
    stopwatch.Stop();

    Console.WriteLine($"   100.000 nesne oluşturma süresi: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine($"   Gen0 Toplama Sayısı: {GC.CollectionCount(0)}");
    Console.WriteLine($"   Gen2 Toplama Sayısı: {GC.CollectionCount(2)}\n");

    // Büyük nesneler
    Console.WriteLine("2️⃣  BÜYÜK NESNELER (100 KB):");
    Console.WriteLine($"   Bellek Öncesi: {GC.GetTotalMemory(false) / 1024_000} MB");

    stopwatch.Restart();
    for (int i = 0; i < 10_000; i++)
    {
        byte[] large = new byte[100_000]; // 100 KB - LOH'da saklanır
    }
    stopwatch.Stop();

    Console.WriteLine($"   10.000 nesne oluşturma süresi: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine($"   Gen0 Toplama Sayısı: {GC.CollectionCount(0)}");
    Console.WriteLine($"   Gen2 Toplama Sayısı: {GC.CollectionCount(2)}");
    Console.WriteLine("   ⚠️  Gen2 toplamaları daha sık tetiklenmiştir!\n");
}

void DemoLOHFragmentation()
{
    Console.WriteLine("🧩 DEMO 2: LOH Parçalanması (Fragmentation)\n");

    Console.WriteLine("Senaryoda büyük nesneler oluştur ve sil:");

    GC.Collect();
    GC.WaitForPendingFinalizers();
    long memBefore = GC.GetTotalMemory(false);

    // Büyük nesneler oluştur ve referans tut
    List<byte[]> largeObjects = new();
    for (int i = 0; i < 100; i++)
    {
        largeObjects.Add(new byte[1_000_000]); // 1 MB'lık nesneler
    }

    long memAfter = GC.GetTotalMemory(false);
    Console.WriteLine($"✓ 100 adet 1MB nesne oluşturuldu");
    Console.WriteLine($"  Bellek artışı: {(memAfter - memBefore) / 1024_000} MB\n");

    // Saçak silme (birkaç nesneyi sil)
    Console.WriteLine("Herbir 2. nesneyi sil (parçalanma oluştur):");
    for (int i = largeObjects.Count - 1; i >= 0; i -= 2)
    {
        largeObjects.RemoveAt(i);
    }

    GC.Collect();
    GC.WaitForPendingFinalizers();

    long memAfterDelete = GC.GetTotalMemory(false);
    Console.WriteLine($"✓ 50 nesne silindi");
    Console.WriteLine($"  Kalan Bellek: {memAfterDelete / 1024_000} MB");
    Console.WriteLine($"  ⚠️  Silinmiş nesnelerin alanları boştur (fragment)!");
    Console.WriteLine($"  💡 Bu boş alanlar kompakt edilmez!\n");

    largeObjects.Clear();
}

void DemoBestPractices()
{
    Console.WriteLine("✨ DEMO 3: En İyi Uygulamalar\n");

    Console.WriteLine("1️⃣  Object Pool Kullanımı (Ayırma sıklığını azalt):");
    var pool = new SimpleObjectPool<byte[]>(() => new byte[100_000], 50);

    var stopwatch = Stopwatch.StartNew();
    for (int i = 0; i < 1_000; i++)
    {
        var obj = pool.Rent();
        // Kullan
        pool.Return(obj);
    }
    stopwatch.Stop();
    Console.WriteLine($"   Object Pool kullanımı: {stopwatch.ElapsedMilliseconds} ms\n");

    Console.WriteLine("2️⃣  ArrayPool Kullanımı (Built-in yönetilen pool):");
    stopwatch.Restart();
    for (int i = 0; i < 1_000; i++)
    {
        var buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(100_000);
        // Kullan
        System.Buffers.ArrayPool<byte>.Shared.Return(buffer);
    }
    stopwatch.Stop();
    Console.WriteLine($"   ArrayPool kullanımı: {stopwatch.ElapsedMilliseconds} ms\n");

    Console.WriteLine("3️⃣  Span<T> / Memory<T> Kullanımı (Stack allocation):");
    stopwatch.Restart();
    for (int i = 0; i < 1_000; i++)
    {
        Span<byte> buffer = stackalloc byte[1_000]; // Stack'ta tahsis edilir
        // Kullan
    }
    stopwatch.Stop();
    Console.WriteLine($"   Span kullanımı: {stopwatch.ElapsedMilliseconds} ms ⚡ (en hızlı)\n");
}

// ===== YARDIMCI SINIFLAR =====

/// <summary>
/// Basit Object Pool uygulaması
/// </summary>
public class SimpleObjectPool<T> where T : class
{
    private readonly Stack<T> _objects = new();
    private readonly Func<T> _factory;

    public SimpleObjectPool(Func<T> factory, int initialCapacity)
    {
        _factory = factory;
        for (int i = 0; i < initialCapacity; i++)
        {
            _objects.Push(_factory());
        }
    }

    public T Rent()
    {
        return _objects.Count > 0 ? _objects.Pop() : _factory();
    }

    public void Return(T obj)
    {
        _objects.Push(obj);
    }
}