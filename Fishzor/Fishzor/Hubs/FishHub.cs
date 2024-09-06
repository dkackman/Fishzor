using Microsoft.AspNetCore.SignalR;
using Fishzor.Services;
using Fishzor.Client.State;

namespace Fishzor.Hubs;

public class FishHub(FishService fishService, ILogger<FishHub> logger) : Hub
{
    private readonly FishService _fishService = fishService;
    private readonly ILogger<FishHub> _logger = logger;

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("New client connected: {ConnectionId}", Context.ConnectionId);
        _fishService.AddFish(Context.ConnectionId);
        await Clients.All.SendAsync("ReceiveFishState", _fishService.ConnectedFish);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        _fishService.RemoveFish(Context.ConnectionId);
        await Clients.All.SendAsync("ReceiveFishState", _fishService.ConnectedFish);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task BroadcastMessage(string message)
    {
        if (_fishService.Fish.TryGetValue(Context.ConnectionId, out var fishState))
        {
            var fishMessage = new FishMessage()
            {
                ClientId = Context.ConnectionId,
                Message = message,
                Color = fishState.Color,
            };
            await Clients.All.SendAsync("ReceiveMessage", fishMessage);
        }
        else
        {
            _logger.LogWarning("Received message to send but source client {ConnectionId} not found", Context.ConnectionId);
        }
    }
}