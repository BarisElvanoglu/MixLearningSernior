using System;
using System.Collections.Generic;

class Program
{
    class Person { public string Name; }

    static void Main()
    {
        // 1. NORMAL LİSTE (Güçlü Referans)
        var normalListe = new List<Person>();
        var p1 = new Person { Name = "Ahmet" };
        normalListe.Add(p1);
        p1 = null; // Yerel referans koptu ama liste Ahmet'i tutuyor

        // 2. WEAK LİSTE (Zayıf Referans)
        var weakListe = new List<WeakReference<Person>>();
        var p2 = new Person { Name = "Mehmet" };
        weakListe.Add(new WeakReference<Person>(p2));
        p2 = null; // Yerel referans koptu, artık Mehmet'i kimse korumuyor

        //--------------------SONUÇ--------------------------//


        Console.WriteLine("--- GC Öncesi ---");
        Console.WriteLine($"Normal Liste[0]: {((Person)normalListe[0]).Name}"); // Ahmet
        Console.WriteLine($"Weak Liste[0] Yaşıyor mu?: {weakListe[0].TryGetTarget(out _)}"); // True

        GC.Collect(); // Çöpçüyü çağırıyoruz
        GC.WaitForPendingFinalizers();

        Console.WriteLine("\n--- GC Sonrası ---");
        Console.WriteLine($"Normal Liste[0]: {((Person)normalListe[0]).Name}"); // Hala Ahmet!

        if (weakListe[0].TryGetTarget(out var p2Target))
            Console.WriteLine($"Weak Liste[0]: {p2Target.Name}");
        else
            Console.WriteLine("Weak Liste[0]: Nesne RAM'den silinmiş (Mehmet uçtu!)");

        Console.ReadLine();
    }
}
}