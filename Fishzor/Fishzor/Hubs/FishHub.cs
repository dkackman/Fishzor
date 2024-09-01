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
        await _fishService.ClientConnected(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await _fishService.ClientDisconnected(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}