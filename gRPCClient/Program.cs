using Grpc.Core;
using Grpc.Net.Client;
using GrpcChatApp;

// Sunucuya bağlan
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
using var channel = GrpcChannel.ForAddress("http://localhost:5005");
var client = new ChatService.ChatServiceClient(channel);

using var call = client.JoinChat();

Console.Write("Kullanıcı Adınız: ");
var user = Console.ReadLine();

// Arka planda gelen mesajları dinle
var readTask = Task.Run(async () => {
    await foreach (var response in call.ResponseStream.ReadAllAsync())
    {
        if (response.User != user)
            Console.WriteLine($"\n[{response.User}]: {response.Message}");
    }
});

// Konsoldan mesaj gönder
while (true)
{
    var msg = Console.ReadLine();
    if (msg == "exit") break;
    await call.RequestStream.WriteAsync(new ChatMessage { User = user, Message = msg });
}

await call.RequestStream.CompleteAsync();