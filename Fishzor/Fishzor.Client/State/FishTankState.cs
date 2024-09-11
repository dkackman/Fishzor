using Microsoft.AspNetCore.SignalR.Client;

namespace Fishzor.Client.State;

public class FishTankState : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private string _hubUrl = string.Empty;
    private IDisposable? _onSubscription;

    public string ClientConnectionId { get; private set; } = string.Empty;
    public IReadOnlyList<FishState> Fish { get; private set; } = [];

    public event Action? OnStateChanged;

    public async Task InitializeAsync(string hubUrl)
    {
        _hubUrl = hubUrl;
        await ConnectToHub();
    }

    private async Task ConnectToHub()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _onSubscription?.Dispose();
        _onSubscription = _hubConnection.On<IEnumerable<FishState>>("ReceiveFishState", fishStates =>
        {
            Fish = fishStates.ToList().AsReadOnly();
            OnStateChanged?.Invoke();
        });

        _hubConnection.On<FishMessage>("ReceiveMessage", async (message) =>
        {
            await DisplayMessageForFish(message.ClientId, message.Message);
            OnMessageReceived?.Invoke(message);
            OnStateChanged?.Invoke();
        });

        _hubConnection.Reconnecting += error =>
        {
            OnStateChanged?.Invoke();
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += connectionId =>
        {
            ClientConnectionId = connectionId ?? string.Empty;
            OnStateChanged?.Invoke();
            return Task.CompletedTask;
        };

        await _hubConnection.StartAsync();
        ClientConnectionId = _hubConnection.ConnectionId!;
    }

    public async ValueTask DisposeAsync()
    {
        _onSubscription?.Dispose();

        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    public event Action<FishMessage>? OnMessageReceived;

    public async Task SendMessageAsync(string message)
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.SendAsync("BroadcastMessage", message);
        }
    }

    public async Task DisplayMessageForFish(string fishId, string message)
    {
        var fish = Fish.FirstOrDefault(f => f.Id == fishId);
        if (fish != null)
        {
            fish.CurrentMessage = message;
            fish.IsMessageVisible = true;
            OnStateChanged?.Invoke();

            await Task.Delay(5000); // Display message for 5 seconds

            fish.IsMessageVisible = false;
            OnStateChanged?.Invoke();
        }
    }
}