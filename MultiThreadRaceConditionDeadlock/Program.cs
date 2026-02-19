using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    // Race Condition Örneği
    private static int sayac = 0;
    private static readonly object kilit = new object();

    // Deadlock Örneği için kaynaklar
    private static readonly object kaynak1 = new object();
    private static readonly object kaynak2 = new object();

    static void Main(string[] args)
    {
        Console.WriteLine("=== RACE CONDITION ÖRNEĞİ ===\n");
        DemonstratRaceCondition();

        Console.WriteLine("\n=== DEADLOCK ÖRNEĞİ ===\n");
        DemonstrateDeadlock();

        Console.WriteLine("\nProgram sona erdi.");
    }

    // Race Condition: Aynı değişkene eşzamanlı erişim
    static void DemonstratRaceCondition()
    {
        sayac = 0;

        Console.WriteLine("Korumasız Erişim (Race Condition):");
        Task[] gorevler = new Task[5];

        for (int i = 0; i < 5; i++)
        {
            gorevler[i] = Task.Run(() =>
            {
                for (int j = 0; j < 10000000; j++)
                {
                    sayac++; // HATA: Veri yarışı oluşur
                }
            });
        }

        Task.WaitAll(gorevler);
        Console.WriteLine($"Beklenen: 50000000, Gerçek: {sayac}\n");

        // Çözüm: Lock kullanarak koruma
        sayac = 0;
        Console.WriteLine("Lock ile Korunan Erişim:");

        gorevler = new Task[5];
        for (int i = 0; i < 5; i++)
        {
            gorevler[i] = Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    lock (kilit) // Kritik bölümü kilitle
                    {
                        sayac++;
                    }
                }
            });
        }

        Task.WaitAll(gorevler);
        Console.WriteLine($"Beklenen: 50000000, Gerçek: {sayac}");
    }

    // Deadlock: Thread'lerin birbirini beklemesi
    static void DemonstrateDeadlock()
    {
        Console.WriteLine("Deadlock Riskli Senaryo:");
        Console.WriteLine("İki thread, iki kaynağı ters sırada kilit altına alıyor...\n");

        bool deadlockOldu = false;

        // Thread 1: Önce kaynak1, sonra kaynak2'yi kilitle
        Task thread1 = Task.Run(() =>
        {
            Console.WriteLine("[Thread 1] Kaynak 1'i kilitlemeye çalışıyor...");
            lock (kaynak1)
            {
                Console.WriteLine("[Thread 1] Kaynak 1 kilitlendi");
                Thread.Sleep(500); // Diğer thread'e fırsat ver

                Console.WriteLine("[Thread 1] Kaynak 2'yi kilitlemeye çalışıyor...");
                if (Monitor.TryEnter(kaynak2, 2000))
                {
                    Console.WriteLine("[Thread 1] Kaynak 2 kilitlendi");
                    Monitor.Exit(kaynak2);
                }
                else
                {
                    Console.WriteLine("[Thread 1] DEADLOCK: Kaynak 2 için zaman aşımı!");
                    deadlockOldu = true;
                }
            }
        });

        // Thread 2: Önce kaynak2, sonra kaynak1'i kilitle (ters sıra!)
        Task thread2 = Task.Run(() =>
        {
            Console.WriteLine("[Thread 2] Kaynak 2'yi kilitlemeye çalışıyor...");
            lock (kaynak2)
            {
                Console.WriteLine("[Thread 2] Kaynak 2 kilitlendi");
                Thread.Sleep(500); // Diğer thread'e fırsat ver

                Console.WriteLine("[Thread 2] Kaynak 1'i kilitlemeye çalışıyor...");
                if (Monitor.TryEnter(kaynak1, 2000))
                {
                    Console.WriteLine("[Thread 2] Kaynak 1 kilitlendi");
                    Monitor.Exit(kaynak1);
                }
                else
                {
                    Console.WriteLine("[Thread 2] DEADLOCK: Kaynak 1 için zaman aşımı!");
                    deadlockOldu = true;
                }
            }
        });

        Task.WaitAll(thread1, thread2);

        Console.WriteLine("\n" + (deadlockOldu ?
            "Deadlock meydana geldi!" :
            "Deadlock kaçınıldı."));

        Console.WriteLine("\n✓ DEADLOCK ÇÖZÜM: Kaynakları AYNI SIRALA kilitlemek");
        Console.WriteLine("  Her zaman Kaynak 1 → Kaynak 2 sırasında kilitle");
    }
}