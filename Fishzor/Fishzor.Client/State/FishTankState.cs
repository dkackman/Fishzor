using Microsoft.AspNetCore.SignalR.Client;

namespace Fishzor.Client.State;

public class FishTankState : IAsyncDisposable
{
    private IEnumerable<FishState> fish = [];
    private HubConnection? _hubConnection;

    public IEnumerable<FishState> Fish
    {
        get => fish;
        private set
        {
            fish = value;
            OnStateChanged?.Invoke();
        }
    }

    public int FishCount
    {
        get => fish.Count();
    }

    public event Action? OnStateChanged;

    public async Task InitializeAsync(string hubUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<IEnumerable<FishState>>("ReceiveFishState", fish =>
        {
            Fish = fish;
        });

        await _hubConnection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}