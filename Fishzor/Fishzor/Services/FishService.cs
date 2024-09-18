using Fishzor.Client.Components;
using Fishzor.Client.State;
using System.Collections.Concurrent;

namespace Fishzor.Services;

public class FishService(ILogger<FishService> logger)
{
    private readonly ILogger<FishService> _logger = logger;
    private readonly ConcurrentDictionary<string, FishState> _connectedFish = new();

    public IEnumerable<FishState> ConnectedFish => _connectedFish.Values;

    public IReadOnlyDictionary<string, FishState> Fish => _connectedFish.AsReadOnly();

    public void AddFish(string connectionId)
    {
        _logger.LogDebug("Client connected: {ConnectionId}", connectionId);

        var scaleValue = Random.Shared.NextDouble() * (1.00 - 0.5) + 0.5;
        var colors = Enum.GetValues(typeof(FishColor));
        var state = new FishState
        {
            Id = connectionId,
            Color = (FishColor)(colors.GetValue(Random.Shared.Next(colors.Length)) ?? FishColor.Orange),
            Scale = scaleValue.ToString("0.##")
        };

        if (!_connectedFish.TryAdd(connectionId, state))
        {
            _logger.LogWarning("Client already exists {ConnectionId}", connectionId);
        }
    }

    public void RemoveFish(string connectionId)
    {
        _logger.LogDebug("Client disconnected: {ConnectionId}", connectionId);
        if (!_connectedFish.TryRemove(connectionId, out _))
        {
            _logger.LogWarning("Instance not found while removing {ConnectionId}", connectionId);
        }
    }
}