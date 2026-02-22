using Grpc.Core;
using GrpcChatApp; // Proto dosyasındaki 'csharp_namespace' ile aynı olmalı

public class ChatRoomService : ChatService.ChatServiceBase
{
    // Bağlı olan istemcileri tutmak için liste
    private static readonly List<IServerStreamWriter<ChatMessage>> _subscribers = new();

    public override async Task JoinChat(IAsyncStreamReader<ChatMessage> requestStream,
                                        IServerStreamWriter<ChatMessage> responseStream,
                                        ServerCallContext context)
    {
        lock (_subscribers) { _subscribers.Add(responseStream); }

        try
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                Console.WriteLine($"[{message.User}]: {message.Message}");

                // Gelen mesajı herkese dağıt
                foreach (var subscriber in _subscribers.ToArray())
                {
                    await subscriber.WriteAsync(message);
                }
            }
        }
        finally
        {
            lock (_subscribers) { _subscribers.Remove(responseStream); }
        }
    }
}