using Fishzor.Client.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Fishzor.Client.State;

public class FishTankState(ILogger<FishTankState> logger) : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private string _hubUrl = string.Empty;
    private IDisposable? _onSubscription;
    private readonly ILogger<FishTankState> _logger = logger;

    public string ClientConnectionId { get; private set; } = string.Empty;
    public IReadOnlyList<FishState> Fish { get; private set; } = [];

    public event Action? OnStateChanged;

    private const int MessageDisplayDurationMS = 10000;

    public async Task InitializeAsync(string hubUrl)
    {
        _logger.LogInformation("Initializing FishTankState with hub URL: {hubUrl}", hubUrl);
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
        _logger.LogDebug("ClientId connected: {ClientConnectionId}", ClientConnectionId);
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

    public async Task SendMessageAsync(ChatMessage message)
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.SendAsync("BroadcastMessage", message);
        }
    }

    public async Task DisplayMessageForFish(string fishId, ChatMessage message)
    {
        _logger.LogDebug("Received message from: {fishId}", fishId);

        var fish = Fish.FirstOrDefault(f => f.Id == fishId);
        if (fish != null)
        {
            fish.CurrentMessage = message;
            fish.IsMessageVisible = true;
            OnStateChanged?.Invoke();

            await Task.Delay(MessageDisplayDurationMS);

            fish.IsMessageVisible = false;
            OnStateChanged?.Invoke();
        }
    }
}