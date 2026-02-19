using System;
using System.Threading;
using System.Threading.Tasks;

class Program

    //volatile anahtar kelimesinin en temel ve en kritik görevi, 
    //bir değişkenin değerinin işlemci çekirdeklerinin kendi içindeki L1, L2 veya L3
    //cache (önbellek) birimlerinde saklanmasını (cache'lenmesini) engellemektir.

{
    // volatile anahtar kelimesi kullanılmayan örnek
    // Bu durumda derleyici değişkeni optimize edebilir ve hatalı sonuçlar üretebilir
    private static bool stopFlag = false;

    // volatile anahtar kelimesi kullanılan örnek
    // Bu derleyiciye değişkeni cache'lemeyecek, her zaman bellekten okuyacak şekilde söyler
    private static volatile bool stopFlagVolatile = false;

    // Sayaç değişkeni
    private static volatile int counter = 0;

    static void Main()
    {
        Console.WriteLine("=== Volatile Anahtar Kelimesi Örneği ===\n");

        // Örnek 1: Volatile olmayan değişken ile sorun gösterimi
        Console.WriteLine("Örnek 1: Volatile Olmayan Değişken (stopFlag)");
        Console.WriteLine("Ana thread 5 saniye sonra stopFlag'i true yapacak...\n");

        Task.Run(() =>
        {
            // Bu loop, derleyici optimizasyonu nedeniyle sonsuz döngüde kalabilir
            // çünkü stopFlag değişkeni cache'te tutulabilir
            int localCounter = 0;
            while (!stopFlag)
            {
                localCounter++;
                Thread.Sleep(100);
            }
            Console.WriteLine($"Worker Thread 1 durdu. Döngü sayısı: {localCounter}");
        });

        Thread.Sleep(5000);
        stopFlag = true;
        Console.WriteLine("Ana Thread: stopFlag = true yapıldı\n");

        Thread.Sleep(2000);

        // Örnek 2: Volatile ile doğru çalışan örnek
        Console.WriteLine("Örnek 2: Volatile Anahtar Kelimesi Kullanılan Değişken (stopFlagVolatile)");
        Console.WriteLine("Ana thread 5 saniye sonra stopFlagVolatile'i true yapacak...\n");

        Task.Run(() =>
        {
            // volatile anahtar kelimesi sayesinde, her iterasyonda
            // bellekten yeni değeri okur ve doğru şekilde çalışır
            int loopCounter = 0;
            while (!stopFlagVolatile)
            {
                loopCounter++;
                Thread.Sleep(100);
            }
            Console.WriteLine($"Worker Thread 2 durdu. Döngü sayısı: {loopCounter}");
        });

        Thread.Sleep(5000);
        stopFlagVolatile = true;
        Console.WriteLine("Ana Thread: stopFlagVolatile = true yapıldı\n");

        Thread.Sleep(2000);

        // Örnek 3: Volatile ile sayaç örneği
        Console.WriteLine("Örnek 3: Counter ile Multi-threading Örneği");
        counter = 0;

        Task[] tasks = new Task[5];
        for (int i = 0; i < 5; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    // volatile sayesinde counter her seferinde bellekten okunur
                    counter++;
                }
            });
        }

        Task.WaitAll(tasks);
        Console.WriteLine($"Son Counter Değeri: {counter}");
        Console.WriteLine("(Beklenen değer: 5000)\n");

        Console.WriteLine("\n=== Özet ===");
        Console.WriteLine("volatile anahtar kelimesi:");
        Console.WriteLine("• Derleyiciye değişkeni cache'lememesini söyler");
        Console.WriteLine("• Her okuma/yazma işleminde bellekten direkt erişim sağlar");
        Console.WriteLine("• Multi-threading uygulamalarında veri tutarlılığı sağlar");
        Console.WriteLine("• Performansi biraz azaltabilir ama güvenliği artırır");
    }
}