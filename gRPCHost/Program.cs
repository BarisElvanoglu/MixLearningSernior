using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;

var builder = WebApplication.CreateBuilder(args);

// 1. gRPC servisini sisteme ekle
builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5005, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});
var app = builder.Build();

// 2. Hazırladığımız ChatService'i haritala
app.MapGrpcService<ChatRoomService>();

Console.WriteLine("gRPC Chat Sunucusu 5005 portunda çalışıyor...");
app.Run();