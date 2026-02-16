using System;
using System.Buffers; // ArrayPool ve MemoryPool için gerekli
using System.Threading.Tasks;

class Program
{
    // 1. STRUCT: Her yerde yaşayabilir (Stack veya Heap).
    // Async metotlarda veya sınıfların içinde saklanabilir.
    public struct StandartStruct
    {
        public int Id;
        public Memory<byte> Veri; // OK: Memory bir struct'tır.
    }

    // 2. REF STRUCT: Sadece Stack'te yaşayabilir.
    // Heap'e asla çıkamaz, bu yüzden çok hızlıdır ama kısıtlıdır.
    public ref struct KisitliStruct
    {
        public int Id;
        public Span<byte> Veri; // OK: Span bir ref struct'tır.
        // public object Obj;   // HATA! Ref struct içinde reference type (heap nesnesi) olamaz.
    }
 
   
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== .NET Modern Bellek Yönetimi Örnekleri ===\n");
        // Küçük, geçici verileri GC'yi yormadan Stack'te ayırır.
        Console.WriteLine("1. stackalloc kullanımı (Stack üzerinde yer açma):");
        //Span referansların tutulduğu yer. stackalloc ile referanslar, newlenmeden Span olarak stackte tutulacak..
        Span<int> stackVerisi = stackalloc int[3] { 10, 20, 30 };// stackte yer açtığı için scope bitince otomatik olarak kaybolur.
        foreach (var sayi in stackVerisi) Console.Write($"{sayi} ");
        Console.WriteLine("\n");
      

        // --- SPAN<T> ---
        // Veriyi kopyalamadan "dilimleme" (Slicing) yapar.
        Console.WriteLine("2. Span<T> ile kopyalamadan dilimleme (Slicing):");
        int[] anaDizi = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        // 3, 4, 5, 6, 7,
        Span<int> pencere = anaDizi.AsSpan(2, 5); // 3. elemandan başla, 5 tane al (KOPYALAMAZ)
        pencere[0] = 999; // Penceredeki değişiklik ana diziyi de değiştirir!
        Console.WriteLine($"Ana dizinin 3. elemanı değişti mi?: {anaDizi[2]}"); // 999 yazar
        Console.WriteLine("");


        // --- MEMORY<T> ---
        // Span'ın aksine Async metotlarda güvenle kullanılabilir.
        Console.WriteLine("3. Memory<T> ve Async kullanımı:");
        Memory<int> bellek = anaDizi.AsMemory();
        await AsyncMetot(bellek);
        Console.WriteLine("");


        // --- ARRAYPOOL<T> ---
        // "Atma, kirala!" mantığı. Büyük dizileri sürekli oluşturup GC'yi yormamak için.
        Console.WriteLine("4. ArrayPool<T> (Kiralama sistemi):");
        byte[] kiralikDizi = ArrayPool<byte>.Shared.Rent(1024); // 1024 byte kiraladık
        try
        {
            // Kiralanan dizi aslında daha büyük olabilir, o yüzden Span ile sınırla!
            Span<byte> guvenliAlan = kiralikDizi.AsSpan(0, 1024);
            guvenliAlan[0] = 1;
            Console.WriteLine($"Havuzdan dizi kiralandı. Gerçek boyut: {kiralikDizi.Length}");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(kiralikDizi); // İADE ETMEK ZORUNLU!
            Console.WriteLine("Dizi havuza iade edildi.");
        }
        Console.WriteLine("");


        // --- MEMORYPOOL<T> ---
        // MemoryPool daha modern ve 'using' ile iade işlemini otomatik yapabilir.
        Console.WriteLine("5. MemoryPool<T> (Modern kiralama):");
        using (IMemoryOwner<char> owner = MemoryPool<char>.Shared.Rent(50))
        {
            Memory<char> mem = owner.Memory;
            mem.Span[0] = 'A';
            Console.WriteLine($"Bellek sahibi (owner) üzerinden işlem yapıldı: {mem.Span[0]}");
        } // 'using' bittiği an bellek otomatik havuza döner.

        Console.WriteLine("\nİşlem tamamlandı. Çıkmak için bir tuşa basın.");
        Console.ReadKey();
    }

    static async Task AsyncMetot(Memory<int> veriler)
    {
        // Span<int> s = veriler.Span; // HATA! Span async içinde doğrudan yaşayamaz.
        await Task.Delay(100); // Bir iş yapılıyor...

        // İşlem yapacağımız zaman .Span diyerek en hızlı haline dönüşüyoruz.
        var span = veriler.Span;
        Console.WriteLine($"Async metot içinden Span ile erişim: {span[0]}");
    }
}


