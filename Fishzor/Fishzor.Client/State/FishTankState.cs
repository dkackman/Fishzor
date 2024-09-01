using Microsoft.AspNetCore.SignalR.Client;

namespace Fishzor.Client.State;

public class FishTankState : IAsyncDisposable
{
    private int _fishCount;
    private HubConnection? _hubConnection;

    public int FishCount
    {
        get => _fishCount;
        private set
        {
            if (_fishCount != value)
            {
                _fishCount = value;
                OnStateChanged?.Invoke();
            }
        }
    }

    public event Action? OnStateChanged;

    public async Task InitializeAsync(string hubUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<int>("ReceiveFishCount", count =>
        {
            FishCount = count;
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