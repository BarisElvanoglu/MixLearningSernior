using System;
using System.Buffers;
using System.Diagnostics;
using System.Linq;

Console.WriteLine("=== stackalloc Nedir? Ne Zaman Kullanılmalı? (Türkçe, Örneklerle) ===\n");

DemoSimpleStackalloc();
Console.WriteLine("\n---\n");
DemoStackallocVsHeap();
Console.WriteLine("\n---\n");
DemoGuardPattern();
Console.WriteLine("\n---\n");
DemoWhenNotToUseStackalloc();

Console.WriteLine("\nBitiş. Bir tuşa basın...");
Console.ReadKey();

static void DemoSimpleStackalloc()
{
    Console.WriteLine("1) Basit kullanım: kısa ömürlü, küçük buffer (stack'te tahsis)\n");

    // küçük, kısa ömürlü buffer: stackalloc tercih edilir
    Span<   int> nums = stackalloc int[16]; // stack'te 16 int alanı
    for (int i = 0; i < nums.Length; i++) nums[i] = i + 1;

    Console.WriteLine("  Toplam: " + nums.ToArray().Sum());
    Console.WriteLine("  Not: Bu Span yalnızca bu metodun stack çerçevesinde geçerlidir.");
    Console.WriteLine("  (async/await veya metod dışına çıkarma yasaktır.)");
}

static void DemoStackallocVsHeap()
{
    Console.WriteLine("2) Performans farkı: stackalloc vs heap (kısa örnek)\n");

    const int iterations = 100_000;
    const int size = 128;

    var sw = Stopwatch.StartNew();
    for (int i = 0; i < iterations; i++)
    {
        Span<byte> s = stackalloc byte[size]; // stack'ta
        s.Fill(0x1);
    }
    sw.Stop();
    Console.WriteLine($"  stackalloc: {sw.ElapsedMilliseconds} ms (GC koleksiyonları yok)");

    sw.Restart();
    int gcBefore = GC.CollectionCount(0);
    for (int i = 0; i < iterations; i++)
    {
        byte[] a = new byte[size]; // heap tahsisi
        Array.Fill(a, (byte)0x1);
    }
    sw.Stop();
    Console.WriteLine($"  new byte[] : {sw.ElapsedMilliseconds} ms (GC0 delta: {GC.CollectionCount(0) - gcBefore})");

    Console.WriteLine("  Çıkarım: stackalloc kısa ömürlü küçük buffer'lar için GC basıncını azaltır ve genellikle daha hızlıdır.");
}

static void DemoGuardPattern()
{
    Console.WriteLine("3) Güvenli desen: küçükse stackalloc, büyükse ArrayPool kullan\n");

    int miktar = 800; // örnek boyut (runtime)
    Console.WriteLine($"  İstenen boyut: {miktar}");

    if (miktar <= 1024) // platforma göre oynanabilir sınır (örnek)
    {
        // stackalloc güvenli (hızlı, GC yok)
        Span<int> buffer = stackalloc int[miktar];
        buffer.Fill(42);
        Console.WriteLine("  stackalloc kullanıldı (local, hızlı). İlk eleman: " + buffer[0]);
    }
    else
    {
        // büyük veya belirsiz boyutlarda heap veya pool kullan
        var pool = ArrayPool<int>.Shared;
        int[] rented = pool.Rent(miktar);
        try
        {
            var span = rented.AsSpan(0, miktar);
            span.Fill(42);
            Console.WriteLine("  ArrayPool kullanıldı (rent). İlk eleman: " + span[0]);
        }
        finally
        {
            pool.Return(rented);
        }
    }

    // Alternatif olarak, tek satırda seçme örneği (C#'ta tip dönüşümleri izin verirse):
    // Span<byte> buf = (miktar <= 1024) ? stackalloc byte[miktar] : new byte[miktar];
    // Yukarıdaki ifade derlenebilir; fakat okunabilirlik ve pool kullanımı için if-else tercih edilir.
}

static void DemoWhenNotToUseStackalloc()
{
    Console.WriteLine("4) Ne zaman KULLANMAMALIYIZ / Riskler ve alternatifler\n");

    Console.WriteLine("  - Büyük veya belirsiz boyutlar (StackOverflow riski).");
    Console.WriteLine("  - Buffer'ın metoda ait ömrü geçmesi gerekiyorsa (ör. return, field) => yasak.");
    Console.WriteLine("  - async/await sırasında buffer canlı kalacaksa => span (stackalloc) kullanılamaz.");

    Console.WriteLine("\n  Örnek: async uyumlu çözüm MemoryPool / ArrayPool ile:");

    // MemoryPool örneği (async ile kullanılmaya uygun)
    using (IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(2048))
    {
        Memory<byte> mem = owner.Memory.Slice(0, 1024); // gerekli kısmı kullan
        mem.Span.Fill(0x7F);
        Console.WriteLine("  MemoryPool ile kiralandı; async/await ile güvenle kullanılabilir.");
    }

    Console.WriteLine("\n  Derleme hatası örneği (yorum):");
    Console.WriteLine("  // Span<int> s = stackalloc int[10];");
    Console.WriteLine("  // return s; // => derleme hatası: stackalloc ile oluşturulan Span metod dışına çıkamaz.");
}