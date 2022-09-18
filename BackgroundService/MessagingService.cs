using System.Text;
using System.Text.Json;
using SuperSimpleTcp;
using Messaging;

class MessagingService : IDisposable
{
    class UnknownMessageTypeException : Exception
    {
        public UnknownMessageTypeException(string type) : base($"Unknown message type: \"{type}\"") { }
    }

    private SimpleTcpServer server;

    public MessagingService(string host = "localhost", int port = 3031)
    {
        server = new SimpleTcpServer($"{host}:{port}");
        server.Events.ClientConnected += ClientConnected;
        server.Events.ClientDisconnected += ClientDisconnected;
        server.Events.DataReceived += DataReceived;
        server.Start();
    }

    public async Task BroadcastAsync(GenericMessage message)
    {
        await BroadcastAsync(message);
    }

    public async Task BroadcastAsync<T>(MessageWithPayload<T> message)
    {
        await BroadcastAsync(message);
    }

    private async Task BroadcastAsync(object message)
    {
        var json = JsonSerializer.Serialize(message);

        await Task.WhenAll(
            server.GetClients()
                  .Select((ipPort) => server.SendAsync(ipPort, json))
        );
    }

    private async Task SendAsync(string ipPort, object message)
    {
        var json = JsonSerializer.Serialize(message);
        await server.SendAsync(ipPort, json);
    }

    public void Dispose()
    {
        if (server.IsListening)
        {
            server.Stop();
        }
        server.Dispose();
    }

    private void ClientConnected(object? sender, ConnectionEventArgs e)
    {
        Console.WriteLine($"[{e.IpPort}] client connected");
    }

    private void ClientDisconnected(object? sender, ConnectionEventArgs e)
    {
        Console.WriteLine($"[{e.IpPort}] client disconnected: {e.Reason}");
    }

    private async void DataReceived(object? sender, DataReceivedEventArgs eventArgs)
    {
        try
        {
            var json = DecodeString(eventArgs);

            var message = JsonSerializer.Deserialize<GenericMessage>(json);
            if (message == null)
            {
                throw new JsonException("Failed to deserialize message");
            }

            if (message.type == "hello")
            {
                await SendAsync(eventArgs.IpPort, new MessageWithPayload<string>("response", "world"));
            } else
            {
                throw new UnknownMessageTypeException(message.type);
            }

        }
        catch (Exception exception)
        {
            await SendAsync(eventArgs.IpPort, new ErrorMessage(exception));
        }
    }

    private String DecodeString(DataReceivedEventArgs e)
    {
        if (e.Data.Array == null)
        {
            return "";
        }

        return Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
    }
}
