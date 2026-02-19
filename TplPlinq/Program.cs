using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        // Test verisi hazırlayalım
        var numbers = Enumerable.Range(1, 10).ToList();

        Console.WriteLine("--- TPL (Parallel.ForEach) Başlıyor ---");
        // TPL Örneği: Her sayı için bir işlem başlatır.
        // İşlem sırası karışık olabilir çünkü çekirdekler hangisini önce bitirirse o ekrana basılır.
        Parallel.ForEach(numbers, number =>
        {
            long result = HeavyComputation(number);
            Console.WriteLine($"TPL: {number}'in karesi {result} (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        });

        Console.WriteLine("\n--- PLINQ (.AsParallel) Başlıyor ---");
        // PLINQ Örneği: Veriyi sorgular ve sonucu bir koleksiyon olarak döner.
        // .AsOrdered() eklemezsek sonuç sırası bozulabilir.
        var parallelResults = numbers.AsParallel()
                                     .AsOrdered() // Sırayı korumak için
                                     .Select(n => new { Number = n, Result = HeavyComputation(n) })
                                     .ToList();

        foreach (var res in parallelResults)
        {
            Console.WriteLine($"PLINQ: {res.Number}'in karesi {res.Result}");
        }

        Console.WriteLine("\nİşlem tamamlandı. Çıkmak için bir tuşa basın.");
        Console.ReadKey();
    }

    // Ağır bir işlemi simüle eden metod
    static long HeavyComputation(int n)
    {
        // İşlemcinin çalıştığını anlamak için kısa bir bekleme ekleyelim
        Thread.Sleep(100);
        return (long)n * n;
    }
}