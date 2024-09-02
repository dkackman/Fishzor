using Microsoft.AspNetCore.SignalR.Client;

namespace Fishzor.Client.State;

public class FishTankState : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    public string ClientConnectionId { get; private set; } = string.Empty;
    public IReadOnlyList<FishState> Fish { get; private set; } = [];

    public event Action? OnStateChanged;

    public async Task InitializeAsync(string hubUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<IEnumerable<FishState>>("ReceiveFishState", fishStates =>
        {
            Fish = fishStates.ToList().AsReadOnly();
            OnStateChanged?.Invoke();
        });

        await _hubConnection.StartAsync();
        ClientConnectionId = _hubConnection.ConnectionId!;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}