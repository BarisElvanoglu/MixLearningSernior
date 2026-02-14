using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("GC Generations Demo (Türkçe)\n");

        PrintOverview("Başlangıç durumu");

        // Kısa ömürlü çok sayıda nesne oluştur (Gen 0'da başlar)
        Console.WriteLine("\n1) Birçok geçici nesne oluşturuluyor (hepsi Gen 0):");
        for (int i = 0; i < 10_000; i++)
        {
            byte[] tmp = new byte[1024]; 
            // kısa ömürlü, hemen kullanımdan çıkıyor
        }
        PrintOverview("Geçici nesneler oluşturulduktan sonra");
        
        // Gen 0 toplaması zorla
        GC.Collect(0, GCCollectionMode.Forced);
        GC.WaitForPendingFinalizers();
        PrintOverview("GC.Collect(0) sonrası");

        // Hayatta kalan nesneleri tutmak için bir dizi oluştur
        Console.WriteLine("\n2) Hayatta kalacak nesneler oluşturuluyor ve referans tutuluyor:");
        object[] survivors = new object[50];
        for (int i = 0; i < survivors.Length; i++)
        {
            survivors[i] = new byte[10_240]; // biraz daha büyük, referans tutuluyor
        }
        PrintOverview("Survivors oluşturulduktan sonra");

        // Gen 0 toplaması yap -> yaşayan nesneler Gen 1'e terfi eder
        GC.Collect(0, GCCollectionMode.Forced);
        GC.WaitForPendingFinalizers();
        PrintOverview("GC.Collect(0) -> yaşayanlar Gen 1'e terfi eder");

        // Nesnelerin hangi generation'da olduğunu göster
        Console.WriteLine($"Örnek survivor nesnesi generation: {GC.GetGeneration(survivors[0])}");

        // Ekstra bir Gen 1 toplaması tetikleyerek yaşayanları Gen 2'ye terfi ettir
        Console.WriteLine("\n3) Gen 1 toplaması tetikleniyor (yaşayanlar Gen 2'ye terfi edebilir):");
        GC.Collect(1, GCCollectionMode.Forced);
        GC.WaitForPendingFinalizers();
        PrintOverview("GC.Collect(1) sonrası");
        Console.WriteLine($"Örnek survivor nesnesi generation: {GC.GetGeneration(survivors[0])}");

        // WeakReference örneği: güçlü referans kaldırılırsa nesne toplanabilir
        Console.WriteLine("\n4) WeakReference örneği (güçlü referans kaldırılır):");
        var strong = new byte[50_000];
        var weak = new WeakReference(strong);
        Console.WriteLine($"WeakReference canlı mı (önce): {weak.IsAlive}");
        strong = null; // güçlü referans kaldırıldı
        GC.Collect(); // tam toplama
        GC.WaitForPendingFinalizers();
        Console.WriteLine($"WeakReference canlı mı (sonra): {weak.IsAlive}");

        Console.WriteLine("\nDemo tamamlandı.");
    }

    static void PrintOverview(string title)
    {
        Console.WriteLine($"\n--- {title} ---");
        Console.WriteLine($"Toplam Bellek (yaklaşık): {GC.GetTotalMemory(false) / 1024} KB");
        Console.WriteLine($"GC Collection Count Gen0: {GC.CollectionCount(0)}");
        Console.WriteLine($"GC Collection Count Gen1: {GC.CollectionCount(1)}");
        Console.WriteLine($"GC Collection Count Gen2: {GC.CollectionCount(2)}");
    }
}