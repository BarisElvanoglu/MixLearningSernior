using System;

// 1. Bileşen Arayüzü (Component)
// Hem temel nesne hem de süslemeler bu arayüzü uygulamalı.
public interface IKahve
{
    string GetDescription();
    double GetCost();
}

// 2. Somut Bileşen (Concrete Component)
// Hiçbir eklemesi olmayan yalın nesne.
public class SadeKahve : IKahve
{
    public string GetDescription() => "Sade Kahve";
    public double GetCost() => 50.0;
}

// 3. Soyut Dekoratör (Decorator)
// Süslemelerin temelini oluşturur. İçinde bir 'IKahve' tutar.
public abstract class KahveDekorator : IKahve
{
    protected IKahve _kahve;
    public KahveDekorator(IKahve kahve) => _kahve = kahve;

    public virtual string GetDescription() => _kahve.GetDescription();
    public virtual double GetCost() => _kahve.GetCost();
}

// 4. Somut Dekoratörler (Concrete Decorators)
// Asıl yetenekleri ekleyen sınıflar.
public class Sut : KahveDekorator
{
    public Sut(IKahve kahve) : base(kahve) { }

    public override string GetDescription() => _kahve.GetDescription() + ", Sütlü";
    public override double GetCost() => _kahve.GetCost() + 10.0; // Süt farkı
}

public class Seker : KahveDekorator
{
    public Seker(IKahve kahve) : base(kahve) { }

    public override string GetDescription() => _kahve.GetDescription() + ", Şekerli";
    public override double GetCost() => _kahve.GetCost() + 5.0; // Şeker farkı
}

// ---------------------------------------------------------
// MAIN - İstemci Kodu (Client)
// ---------------------------------------------------------
class Program
{
    static void Main(string[] args)
    {
        // 1. Önce sade bir kahve alalım
        IKahve siparis = new SadeKahve();
        Console.WriteLine($"{siparis.GetDescription()} -> Fiyat: {siparis.GetCost()} TL");

        // 2. Şimdi içine süt ekleyelim (Sarmalıyoruz)
        siparis = new Sut(siparis);
        Console.WriteLine($"{siparis.GetDescription()} -> Fiyat: {siparis.GetCost()} TL");

        // 3. Bir de şeker patlatalım (Tekrar sarmalıyoruz)
        siparis = new Seker(siparis);
        Console.WriteLine($"{siparis.GetDescription()} -> Fiyat: {siparis.GetCost()} TL");

        // Sonuç: "Sade Kahve, Sütlü, Şekerli -> Fiyat: 65 TL"
    }
}