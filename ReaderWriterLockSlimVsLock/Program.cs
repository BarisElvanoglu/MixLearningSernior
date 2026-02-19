using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

// ============================================================
// READERWRITERLOCKSLIM vs LOCK KARŞILAŞTIRMASI
// ============================================================
// lock: Tüm erişimleri (okuma/yazma) seri hale getirir
// ReaderWriterLockSlim: Okumalar paralel, yazmalar seri
// ============================================================

class Program
{
    // Test için veri tabanı simulasyonu
    private static int bakanSayisi = 0;

    static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  ReaderWriterLockSlim vs Lock Performans Karşılaştırması  ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

        // 1. Lock örneği
        Console.WriteLine("1️⃣  LOCK KULLANARAK TEST\n");
        TestWithLock();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 2. ReaderWriterLockSlim örneği
        Console.WriteLine("2️⃣  ReaderWriterLockSlim KULLANARAK TEST\n");
        TestWithReaderWriterLockSlim();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 3. Detaylı karşılaştırma
        Console.WriteLine("3️⃣  DETAYLI ANALIZ\n");
        ComparisonAnalysis();
    }

    // ============================================================
    // LOCK ÖRNEĞI - Tüm erişimler seri (hepsi aynı şekilde bekleme)
    // ============================================================
    static void TestWithLock()
    {
        var lockObject = new object();
        int veri = 0;
        var stopwatch = Stopwatch.StartNew();

        // 10 okuyucu thread (okuma işlemi daha sık)
        var okuycuGorevleri = new Task[10];
        for (int i = 0; i < 10; i++)
        {
            int threadId = i;
            okuycuGorevleri[i] = Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    lock (lockObject) // Kilit alması gerekli
                    {
                        var degeri_oku = veri; // Okuma işlemi
                        Thread.SpinWait(10); // Okuma simulasyonu
                    }
                }
            });
        }

        // 2 yazıcı thread (yazma işlemi daha nadir)
        var yazicGorevleri = new Task[2];
        for (int i = 0; i < 2; i++)
        {
            int threadId = i + 10;
            yazicGorevleri[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    lock (lockObject) // Kilit alması gerekli (okurlar bloke olur)
                    {
                        veri++; // Yazma işlemi
                        Thread.SpinWait(50); // Yazma simulasyonu
                    }
                }
            });
        }

        Task.WaitAll(okuycuGorevleri.Concat(yazicGorevleri).ToArray());
        stopwatch.Stop();

        Console.WriteLine($"📊 Lock Sonucu:");
        Console.WriteLine($"   • Son değer: {veri}");
        Console.WriteLine($"   ⏱️  Süre: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"   ⚠️  Not: Okurlar da yazarlar kadar bekler (verimsiz)");
    }

    // ============================================================
    // READERWRITERLOCKSLIM ÖRNEĞI - Okumalar paralel, yazmalar seri
    // ============================================================
    static void TestWithReaderWriterLockSlim()
    {
        var rwLock = new ReaderWriterLockSlim();
        int veri = 0;
        var stopwatch = Stopwatch.StartNew();

        // 10 okuyucu thread (paralel çalışır)
        var okuycuGorevleri = new Task[10];
        for (int i = 0; i < 10; i++)
        {
            int threadId = i;
            okuycuGorevleri[i] = Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    rwLock.EnterReadLock(); // Okuma kilidi
                    try
                    {
                        var degeri_oku = veri; // Okuma işlemi
                        Thread.SpinWait(10); // Okuma simulasyonu
                    }
                    finally
                    {
                        rwLock.ExitReadLock();
                    }
                }
            });
        }

        // 2 yazıcı thread (seri çalışır, okurlar bloke olur)
        var yazicGorevleri = new Task[2];
        for (int i = 0; i < 2; i++)
        {
            int threadId = i + 10;
            yazicGorevleri[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    rwLock.EnterWriteLock(); // Yazma kilidi
                    try
                    {
                        veri++; // Yazma işlemi
                        Thread.SpinWait(50); // Yazma simulasyonu
                    }
                    finally
                    {
                        rwLock.ExitWriteLock();
                    }
                }
            });
        }

        Task.WaitAll(okuycuGorevleri.Concat(yazicGorevleri).ToArray());
        stopwatch.Stop();
        rwLock.Dispose();

        Console.WriteLine($"📊 ReaderWriterLockSlim Sonucu:");
        Console.WriteLine($"   • Son değer: {veri}");
        Console.WriteLine($"   ⏱️  Süre: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"   ✅ Not: Okurlar paralel çalışır (daha verimli)");
    }

    // ============================================================
    // DETAYLI KARŞILAŞTıRMA - Farkları gösterir
    // ============================================================
    static void ComparisonAnalysis()
    {
        Console.WriteLine("📋 ÖZET VE SEÇIM KRİTERLERİ:\n");

        Console.WriteLine("🔒 LOCK Ne Zaman Kullanılır?");
        Console.WriteLine("   ✓ Basit senaryolarda (hem okuma hem yazma az)");
        Console.WriteLine("   ✓ Kodun sadeleştirilmesi önemli");
        Console.WriteLine("   ✓ Lock karmaşıklığı riskliyse");
        Console.WriteLine("   ✓ Yazma işlemleri sık olduğunda\n");

        Console.WriteLine("📖 ReaderWriterLockSlim Ne Zaman Kullanılır?");
        Console.WriteLine("   ✓ Okuma işlemleri yazma işlemlerinden çok daha sık");
        Console.WriteLine("   ✓ Performans kritik (veri tabanı okuma gibi)");
        Console.WriteLine("   ✓ Çok sayıda okuyucu, az sayıda yazıcı");
        Console.WriteLine("   ✓ Cache, configuration, lookup tablolar\n");

        Console.WriteLine("⚙️  Kod Örneği Farkları:\n");

        Console.WriteLine("LOCK:");
        Console.WriteLine(@"
    lock (kilit)
    {
        // Hem okuma hem yazma
        var deger = veri;
        veri = deger + 1;
    }");

        Console.WriteLine("\nREADERWRITERLOCKSLIM:");
        Console.WriteLine(@"
    // OKUMA
    rwLock.EnterReadLock();
    try { var deger = veri; }
    finally { rwLock.ExitReadLock(); }
    
    // YAZMA
    rwLock.EnterWriteLock();
    try { veri++; }
    finally { rwLock.ExitWriteLock(); }");

        Console.WriteLine("\n📊 Performans İpuçları:");
        Console.WriteLine("   • Lock: Okuma yoğun sistemlerde daha yavaş");
        Console.WriteLine("   • ReaderWriterLockSlim: Okuma yoğun sistemlerde hızlı");
        Console.WriteLine("   • ReaderWriterLockSlim: Yazma sırasında deadlock riski daha yüksek");
        Console.WriteLine("   • Seçim her zaman test ve profiling ile yapılmalı");
    }
}