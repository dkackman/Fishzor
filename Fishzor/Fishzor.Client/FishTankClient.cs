using Fishzor.Client.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Fishzor.Client.State;

namespace Fishzor.Client;

public class FishTankClient(ILogger<FishTankClient> logger) : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private IDisposable? _onReceiveFishStateSubscription;
    private IDisposable? _onReceiveMessageSubscription;
    private readonly ILogger<FishTankClient> _logger = logger;
    private Dictionary<string, FishState> _fish = [];
    public IReadOnlyCollection<FishState> Fish => _fish.Values;
    public string ClientConnectionId { get; private set; } = string.Empty;
    public bool IsOfflineMode { get; set; } = false;

    public event Action? OnStateChanged;

    public async Task InitializeAsync(string hubUrl)
    {
        try
        {
            if (_hubConnection is not null)
            {
                await DisposeAsync();
            }

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _onReceiveFishStateSubscription?.Dispose();
            _onReceiveFishStateSubscription = _hubConnection.On<IEnumerable<FishState>>("ReceiveFishState", fishStates =>
            {
                _fish = fishStates.ToDictionary(f => f.Id);
                OnStateChanged?.Invoke();
            });

            _onReceiveMessageSubscription?.Dispose();
            _onReceiveMessageSubscription = _hubConnection.On<FishMessage>("ReceiveMessage", (message) =>
            {
                DisplayMessageForFish(message.ClientId, message.Message);
                OnMessageReceived?.Invoke(message);
                OnStateChanged?.Invoke();
            });

            _hubConnection.Reconnecting += error =>
            {
                IsOfflineMode = true;
                OnStateChanged?.Invoke();
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                IsOfflineMode = false;
                ClientConnectionId = connectionId ?? string.Empty;
                OnStateChanged?.Invoke();
                return Task.CompletedTask;
            };

            _hubConnection.Closed += error =>
            {
                IsOfflineMode = false;
                OnStateChanged?.Invoke();
                return Task.CompletedTask;
            };

            await _hubConnection.StartAsync();
            IsOfflineMode = false;
            ClientConnectionId = _hubConnection.ConnectionId!;
            _logger.LogDebug("ClientId connected: {ClientConnectionId}", ClientConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to the hub. Working in offline mode.");
            IsOfflineMode = true;
            ClientConnectionId = "offline1";
            // Initialize with some default fish for offline mode
            _fish = new Dictionary<string, FishState>()
            {
                ["offline1"] = new FishState { Id = ClientConnectionId, Color = FishColor.Orange, Scale = "1.0" },
            };
        }
        finally
        {
            OnStateChanged?.Invoke();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _onReceiveFishStateSubscription?.Dispose();
        _onReceiveMessageSubscription?.Dispose();

        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }

    public event Action<FishMessage>? OnMessageReceived;

    public async Task SendMessageAsync(ChatMessage message)
    {
        if (_hubConnection is not null && !IsOfflineMode)
        {
            await _hubConnection.SendAsync("BroadcastMessage", message);
        }
        else
        {
            _logger.LogWarning("Cannot send message: not connected to hub");
        }
    }

    private void DisplayMessageForFish(string fishId, ChatMessage message)
    {
        _logger.LogDebug("Received message from: {fishId}", fishId);

        if (_fish.TryGetValue(fishId, out var fish))
        {
            fish.CurrentMessage = message;
        }
    }
}