using Microsoft.AspNetCore.SignalR;
using Fishzor.Hubs;
using System.Collections.Concurrent;
using Fishzor.Client.State;
using Fishzor.Client.Components;

namespace Fishzor.Services;

public class FishService(IHubContext<FishHub> hubContext, ILogger<FishService> logger)
{
    private static readonly Random _random = new();

    private readonly IHubContext<FishHub> _hubContext = hubContext;
    private readonly ILogger<FishService> _logger = logger;
    private readonly ConcurrentDictionary<string, FishState> _connectedClients = new();

    public async Task ClientConnected(string connectionId)
    {
        _logger.LogDebug("Client connected: {ConnectionId}", connectionId);
        
        var colors = Enum.GetValues(typeof(FishColor));
        var state = new FishState 
        { 
            Id = connectionId,
            Color = (FishColor)(colors.GetValue(_random.Next(colors.Length)) ?? FishColor.Orange)
        };
        _connectedClients.TryAdd(connectionId, state);
        await NotifyClients();
    }

    public async Task ClientDisconnected(string connectionId)
    {
        _logger.LogDebug("Client disconnected: {ConnectionId}", connectionId);
        _connectedClients.TryRemove(connectionId, out _);
        await NotifyClients();
    }

    private async Task NotifyClients()
    {
        _logger.LogDebug("Notifying clients of updated fish count: {FishCount}", _connectedClients.Count);
        await _hubContext.Clients.All.SendAsync("ReceiveFishCount", _connectedClients.Count);
        await _hubContext.Clients.All.SendAsync("ReceiveFishState", _connectedClients.Values);
    }
}