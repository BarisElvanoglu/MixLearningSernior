using Moq; // Moq kütüphanesi
using NSubstitute; // NSubstitute kütüphanesi

class Program
{
    static void Main(string[] args)
    {

        // --- 1. Moq Örneği ---
        // Bir mock nesnesi oluşturulur (.Object ile asıl nesneye erişilir)
        var moqRepo = new Mock<IOrderRepository>();

        // Davranış tanımlama (Setup)
        moqRepo.Setup(x => x.SaveOrder(101)).Returns(true);
        moqRepo.Setup(x => x.SaveOrder(999)).Returns(false);

        var serviceWithMoq = new OrderService(moqRepo.Object);
        Console.WriteLine($"Moq (101): {serviceWithMoq.Process(101)}"); // Çıktı: Başarılı
        Console.WriteLine($"Moq (999): {serviceWithMoq.Process(999)}"); // Çıktı: Hata Oluştu


        // --- 2. NSubstitute Örneği ---
        // Doğrudan arayüz üzerinden nesne oluşturulur (Daha sade)
        var nSubRepo = Substitute.For<IOrderRepository>();

        // Davranış tanımlama (Doğal dil gibi)
        nSubRepo.SaveOrder(202).Returns(true);
        nSubRepo.SaveOrder(555).Returns(false);

        var serviceWithNSub = new OrderService(nSubRepo);
        Console.WriteLine($"NSub (202): {serviceWithNSub.Process(202)}"); // Çıktı: Başarılı
        Console.WriteLine($"NSub (555): {serviceWithNSub.Process(555)}"); // Çıktı: Hata Oluştu

        // Doğrulama (Verifying) - Metot gerçekten çağrıldı mı?
        nSubRepo.Received().SaveOrder(202);
    }
}