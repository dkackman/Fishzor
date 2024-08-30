using Microsoft.AspNetCore.SignalR;
using Fishzor.Services;
using Microsoft.Extensions.Logging;

namespace Fishzor.Hubs;

public class FishHub : Hub
{
    private readonly FishService _fishService;
    private readonly ILogger<FishHub> _logger;

    public FishHub(FishService fishService, ILogger<FishHub> logger)
    {
        _fishService = fishService;
        _logger = logger;
        _logger.LogDebug("FishHub instance created");
    }

    public async Task AddFish()
    {
        _logger.LogInformation("AddFish request received");
        int newCount = await _fishService.AddFish();
        await Clients.All.SendAsync("ReceiveFishCount", newCount);
        _logger.LogDebug("Fish added. New count: {NewCount}", newCount);
    }

    public async Task RemoveFish()
    {
        _logger.LogInformation("RemoveFish request received");
        int newCount = await _fishService.RemoveFish();
        await Clients.All.SendAsync("ReceiveFishCount", newCount);
        _logger.LogDebug("Fish removed. New count: {NewCount}", newCount);
    }

    public async Task GetFishCount()
    {
        _logger.LogInformation("GetFishCount request received");
        int count = _fishService.GetFishCount();
        await Clients.Caller.SendAsync("ReceiveFishCount", count);
        _logger.LogDebug("Fish count sent to caller: {Count}", count);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("New client connected: {ConnectionId}", Context.ConnectionId);
        await GetFishCount();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        if (exception != null)
        {
            _logger.LogError(exception, "Client disconnected with error");
        }
        await base.OnDisconnectedAsync(exception);
    }
}