using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq; // Unit test için Mocking kütüphanesi
using System;
using System.Threading.Tasks;
using UnitIntegrationFunctionalTest;
using Xunit;
// --- 1. UYGULAMA KODU (HEDEF) ---


// --- 2. UNIT TEST (Birim Testi) ---
// Sadece ApplyDiscount metodundaki matematiksel mantığı test eder.
public class OrderUnitTests
{
    [Fact]
    public async Task ProcessOrder_PriceOver100_AppliesTenPercentDiscount()
    {
        // 1. ARRANGE (Hazırlık)
        // Gerçek bir veritabanı yerine sahte (Mock) bir repo oluşturuyoruz
        var mockRepo = new Mock<IRepository>();

        // Save metodu çağrıldığında her zaman 'true' dönmesini sağlıyoruz
        mockRepo.Setup(repo => repo.Save(It.IsAny<Order>()))
                .ReturnsAsync(true);

        var service = new OrderService(mockRepo.Object);
        var order = new Order { Id = 1, Price = 200m };

        // 2. ACT (Çalıştırma)
        var result = await service.ProcessOrder(order);

        // 3. ASSERT (Doğrulama)
        // Fiyat 200 idi, %10 indirimle 180 olmalı
        Assert.Equal(180m, order.Price);

        // Save metodunun tam olarak 1 kere çağrıldığını doğrula
        mockRepo.Verify(repo => repo.Save(It.IsAny<Order>()), Times.Once);

        Assert.True(result);
    }

    [Fact]
    public async Task ProcessOrder_PriceUnder100_NoDiscountApplied()
    {
        // Arrange
        var mockRepo = new Mock<IRepository>();
        mockRepo.Setup(r => r.Save(It.IsAny<Order>())).ReturnsAsync(true);

        var service = new OrderService(mockRepo.Object);
        var order = new Order { Id = 2, Price = 50m };

        // Act
        await service.ProcessOrder(order);

        // Assert
        // Fiyat 100'den küçük olduğu için 50 kalmalı
        Assert.Equal(50m, order.Price);
    }
}

// --- 3. INTEGRATION TEST (Entegrasyon Testi) ---
// Kodun gerçek bir veritabanı (InMemory) ile düzgün çalışıp çalışmadığını test eder.
public class OrderIntegrationTests
{
    [Fact]
    public async Task ProcessOrder_ShouldSucceed_WhenDatabaseIsReady()
    {
        // Gerçek bir DbContext (InMemory) kurulumu
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb").Options;
        var repo = new SqlRepository(new AppDbContext(options));
        var service = new OrderService(repo);

        var result = await service.ProcessOrder(new Order { Id = 1, Price = 150 });

        Assert.True(result); // Veritabanına başarıyla yazıldı mı?
    }
}

// --- 4. FUNCTIONAL TEST (Fonksiyonel Test) ---
// Kullanıcının "Sipariş Ver" butonuna basması gibi tüm sistemi (API + Servis + DB) test eder.
public class OrderFunctionalTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    // WebApplicationFactory otomatik olarak uygulamayı 'test server' olarak başlatır.
    public OrderFunctionalTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(); // Kırmızı yanan yerin yerine bunu kullanıyoruz
    }

    [Fact]
    public async Task FullOrderFlow_ShouldWork()
    {
        // Act: Gerçek bir HTTP isteği atıyoruz
        var response = await _client.PostAsync("/api/order", null);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
}