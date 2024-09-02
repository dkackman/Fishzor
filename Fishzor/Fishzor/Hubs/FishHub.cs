using Microsoft.AspNetCore.SignalR;
using Fishzor.Services;

namespace Fishzor.Hubs;

public class FishHub(FishService fishService, ILogger<FishHub> logger) : Hub
{
    private readonly FishService _fishService = fishService;
    private readonly ILogger<FishHub> _logger = logger;

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("New client connected: {ConnectionId}", Context.ConnectionId);
        _fishService.AddFish(Context.ConnectionId);
        await NotifyClients();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        _fishService.RemoveFish(Context.ConnectionId);
        await NotifyClients();
        await base.OnDisconnectedAsync(exception);
    }

    private async Task NotifyClients()
    {
        _logger.LogDebug("Notifying clients of updated fish");
        await Clients.All.SendAsync("ReceiveFishState", _fishService.ConnectedFish);
    }
}