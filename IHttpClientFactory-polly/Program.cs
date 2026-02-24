using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using System.Net;

class Program
{
    static async Task Main(string[] args)
    {

        // --- ESKİ VE TEHLİKELİ YÖNTEM (Socket Exhaustion Riski) ---
        // Her istekte 'new' kullanmak soketleri açık bırakır.

        //for (int i = 0; i < 100; i++)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var result = await client.GetAsync("https://api.example.com");
        //    }
        //}


        // --- YENİ VE GÜVENLİ YÖNTEM (IHttpClientFactory + Polly) ---

        // 1. Servis Kaydı ve Konfigürasyon
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddHttpClient("MySafeClient", client =>
        {
            client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
            client.Timeout = TimeSpan.FromSeconds(5);
        })
        // 2. Polly ile Retry (Yeniden Deneme) Politikası Ekleme
        .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(new[]
        {
            TimeSpan.FromSeconds(1), // 1. deneme öncesi bekleme
            TimeSpan.FromSeconds(2), // 2. deneme öncesi bekleme
            TimeSpan.FromSeconds(5)  // 3. deneme öncesi bekleme
        }, (result, timeSpan, retryCount, context) => {
            Console.WriteLine($"Hata alındı! {retryCount}. deneme yapılıyor... (Bekleme: {timeSpan.TotalSeconds}s)");
        }));

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        // 3. Kullanım
        var httpClient = clientFactory.CreateClient("MySafeClient");

        try
        {
            Console.WriteLine("İstek gönderiliyor...");
            var response = await httpClient.GetAsync("posts/1"); // Geçerli adres

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Veri başarıyla alındı.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Tüm denemelere rağmen başarısız: {ex.Message}");
        }
    }
}


