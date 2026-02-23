using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
// 1. Bağlantıyı Yapılandır
var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5000/borsa") // Sunucu adresinden emin olun
    .WithAutomaticReconnect()
    .Build();

// 2. Sunucudan gelecek "FiyatGuncelle" komutunu dinle
// NOT: Metot ismi büyük 'O' ile başlar -> .On<T1, T2>
connection.On<string, int>("FiyatGuncelle", (sembol, fiyat) => {
    Console.WriteLine($"💰 {sembol} Yeni Fiyat: {fiyat:N0} TL - Zaman: {DateTime.Now:HH:mm:ss}");
});

try
{
    // 3. Başlat
    await connection.StartAsync();
    Console.WriteLine("🚀 Borsaya bağlanıldı. Fiyat akışı başladı...");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Bağlantı hatası: {ex.Message}");
}

// Uygulamanın kapanmasını engellemek için bekletiyoruz
Console.WriteLine("\n--- Çıkış yapmak için ENTER tuşuna basın ---\n");
Console.ReadLine();

// Temiz bir kapanış için
await connection.StopAsync();