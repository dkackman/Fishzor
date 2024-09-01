using Microsoft.AspNetCore.SignalR;
using Fishzor.Hubs;
using System.Collections.Concurrent;

namespace Fishzor.Services;

public class FishService(IHubContext<FishHub> hubContext, ILogger<FishService> logger)
{
    private readonly IHubContext<FishHub> _hubContext = hubContext;
    private readonly ILogger<FishService> _logger = logger;
    private readonly ConcurrentDictionary<string, bool> _connectedClients = new();

    public async Task ClientConnected(string connectionId)
    {
        _logger.LogDebug("Client connected: {ConnectionId}", connectionId);
        _connectedClients.TryAdd(connectionId, true);
        await NotifyClients(_connectedClients.Count);
    }

    public async Task ClientDisconnected(string connectionId)
    {
        _logger.LogDebug("Client disconnected: {ConnectionId}", connectionId);
        _connectedClients.TryRemove(connectionId, out _);
        await NotifyClients(_connectedClients.Count);
    }

    private async Task NotifyClients(int fishCount)
    {
        _logger.LogDebug("Notifying clients of updated fish count: {FishCount}", fishCount);
        await _hubContext.Clients.All.SendAsync("ReceiveFishCount", fishCount);
    }
}