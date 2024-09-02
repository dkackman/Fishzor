using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;

namespace Fishzor.Client.State;

public class FishTankState : IAsyncDisposable
{
    private List<FishState> _fish = new();
    private HubConnection? _hubConnection;
    public string? ClientConnectionId { get; private set; }
    public IReadOnlyList<FishState> Fish => _fish.AsReadOnly();

    public event Action? OnStateChanged;

    public async Task InitializeAsync(string hubUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<IEnumerable<FishState>>("ReceiveFishState", fishStates =>
        {
            _fish = new List<FishState>(fishStates);
            OnStateChanged?.Invoke();
        });

        await _hubConnection.StartAsync();
        ClientConnectionId = _hubConnection.ConnectionId;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}