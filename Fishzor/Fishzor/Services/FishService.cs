using System.Collections.Concurrent;
using Fishzor.Client.State;
using Fishzor.Client.Components;

namespace Fishzor.Services;

public class FishService(ILogger<FishService> logger)
{
    private static readonly Random _random = new();

    private readonly ILogger<FishService> _logger = logger;
    private readonly ConcurrentDictionary<string, FishState> _connectedFish = new();
    public IEnumerable<FishState> ConnectedFish => _connectedFish.Values;

    public IReadOnlyDictionary<string, FishState> Fish => _connectedFish.AsReadOnly();

    public void AddFish(string connectionId)
    {
        _logger.LogDebug("Client connected: {ConnectionId}", connectionId);

        var scaleValue = _random.NextDouble() * (1.00 - 0.5) + 0.5;
        var colors = Enum.GetValues(typeof(FishColor));
        var state = new FishState
        {
            Id = connectionId,
            Color = (FishColor)(colors.GetValue(_random.Next(colors.Length)) ?? FishColor.Orange),
            Scale = scaleValue.ToString("0.##")
        };
        _connectedFish.TryAdd(connectionId, state);
    }

    public void RemoveFish(string connectionId)
    {
        _logger.LogDebug("Client disconnected: {ConnectionId}", connectionId);
        _connectedFish.TryRemove(connectionId, out _);
    }
}