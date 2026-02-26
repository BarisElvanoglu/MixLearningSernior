using System;

// 1. Ürün Arayüzü (Product Interface)
// Tüm taşıma yöntemlerinin uyması gereken standart kural seti.
public interface ITransport
{
    string Deliver();
}

// 2. Somut Ürünler (Concrete Products)
// Arayüzü uygulayan gerçek nesneler.
public class Truck : ITransport
{
    public string Deliver() => "Paket kara yoluyla (Tır ile) teslim ediliyor.";
}

public class Ship : ITransport
{
    public string Deliver() => "Paket deniz yoluyla (Gemi ile) teslim ediliyor.";
}

// 3. Creator (Creator) - Fabrika Sınıfı
// Nesne üretim kararının verildiği merkez üssü.
public abstract class Logistics
{
    // Factory Method: Alt sınıflar bu metodu override ederek kendi nesnelerini döner.
    public abstract ITransport CreateTransport();

    // Fabrika sadece nesne üretmez, o nesne üzerinden iş mantığı da yürütebilir.
    public void PlanDelivery()
    {
        var transport = CreateTransport();
        Console.WriteLine($"Lojistik Planı: {transport.Deliver()}");
    }
}

// 4. Somut Fabrikalar (Concrete Creators)
public class RoadLogistics : Logistics
{
    public override ITransport CreateTransport() => new Truck();
}

public class SeaLogistics : Logistics
{
    public override ITransport CreateTransport() => new Ship();
}

// ---------------------------------------------------------
// MAIN - İstemci Kodu (Client)
// ---------------------------------------------------------
class Program
{
    static void Main(string[] args)
    {
        Logistics logistics;

        // Senaryo: Uygulama çalışma anında (örneğin kullanıcı seçimine göre) 
        // hangi taşıma türünün kullanılacağına karar veriyor.

        Console.WriteLine("Taşıma yöntemi seçiniz (Road/Sea):");
        string choice = "Road"; // Normalde kullanıcıdan veya config'den gelir

        if (choice == "Road")
            logistics = new RoadLogistics();
        else
            logistics = new SeaLogistics();

        // Client (Main), Truck veya Ship sınıflarını doğrudan tanımaz.
        // Sadece 'logistics' üzerinden işini halleder.
        logistics.PlanDelivery();
    }
}