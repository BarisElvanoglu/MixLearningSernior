using System;
using System.Collections.Generic;

class Program
{
    class Person { public string Name; }

    static void Main()
    {
        var weakListe = new List<WeakReference<Person>>();
        var liste = new List<Person>();
        // İşlemi ayrı bir metodda yapıyoruz ki referans metod bitince ölsün
        NesneOlusturVeEkle(weakListe);
        NesneOlustur(liste);
        Console.WriteLine("--- Temizlik Başlıyor ---");

        // Kesin temizlik protokolü
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        if (weakListe[0].TryGetTarget(out var target))
            Console.WriteLine("Hala yaşıyor: " + target.Name);
        else
            Console.WriteLine("Mehmet nihayet silindi!");

        Console.WriteLine(liste[0].Name);
        Console.ReadLine();
    }

    static void NesneOlusturVeEkle(List<WeakReference<Person>> liste)
    {
        var p2 = new Person { Name = "Mehmet" };
        liste.Add(new WeakReference<Person>(p2));
        // Metod bittiği an p2 yerel referansı ölür.
    }
    static void NesneOlustur(List<Person> liste1)
    {
        var p1 = new Person { Name = "Ahmet" };
        liste1.Add(p1);
        // Metod bittiği an p2 yerel referansı ölür.
    }
}
