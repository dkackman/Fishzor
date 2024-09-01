using Microsoft.AspNetCore.SignalR;
using Fishzor.Hubs;
using System.Collections.Concurrent;

namespace Fishzor.Services;

public class FishService
{
    private readonly IHubContext<FishHub> _hubContext;
    private readonly ILogger<FishService> _logger;
    private readonly ConcurrentDictionary<string, bool> _connectedClients = new();

    public FishService(IHubContext<FishHub> hubContext, ILogger<FishService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
        _logger.LogInformation("FishService initialized");
    }

    public async Task ClientConnected(string connectionId)
    {
        _connectedClients.TryAdd(connectionId, true);
        await UpdateFishCount();
    }

    public async Task ClientDisconnected(string connectionId)
    {
        _connectedClients.TryRemove(connectionId, out _);
        await UpdateFishCount();
    }

    public async Task UpdateFishCount()
    {
        int fishCount = _connectedClients.Count;
        _logger.LogInformation("Fish count updated: {FishCount}", fishCount);
        await NotifyClients(fishCount);
    }

    private async Task NotifyClients(int fishCount)
    {
        _logger.LogDebug("Notifying clients of updated fish count: {FishCount}", fishCount);
        await _hubContext.Clients.All.SendAsync("ReceiveFishCount", fishCount);
    }
}