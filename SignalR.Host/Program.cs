using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<BorsaHub>("/borsa");

// Arka plan görevi
// app.Run() öncesi Task.Run kullanmak yerine, 
// app.Lifetime'ı kullanarak uygulama başladığında döngüyü tetikliyoruz.
_ = Task.Run(async () =>
{
    // Uygulamanın tam olarak başlamasını bekliyoruz
    await Task.Delay(1000);

    var rng = new Random();
    // HubContext'i uygulama servislerinden alıyoruz
    var hubContext = app.Services.GetRequiredService<IHubContext<BorsaHub>>();

    while (true)
    {
        int fiyat = rng.Next(60000, 70000);

        // Tüm istemcilere mesaj gönder
        await hubContext.Clients.All.SendAsync("FiyatGuncelle", "BTC", fiyat);

        // 2 saniye bekle
        await Task.Delay(2000);
    }
});

app.Run();

public class BorsaHub : Hub { }