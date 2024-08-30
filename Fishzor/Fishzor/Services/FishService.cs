using Microsoft.AspNetCore.SignalR;
using Fishzor.Hubs;
using Microsoft.Extensions.Logging;

namespace Fishzor.Services;

public class FishService
{
    private int _fishCount = 0;
    private readonly IHubContext<FishHub> _hubContext;
    private readonly ILogger<FishService> _logger;

    public FishService(IHubContext<FishHub> hubContext, ILogger<FishService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
        _logger.LogInformation("FishService initialized");
    }

    public async Task<int> AddFish()
    {
        _fishCount++;
        _logger.LogInformation("Fish added. New count: {FishCount}", _fishCount);
        await NotifyClients();
        return _fishCount;
    }

    public async Task<int> RemoveFish()
    {
        if (_fishCount > 0)
        {
            _fishCount--;
            _logger.LogInformation("Fish removed. New count: {FishCount}", _fishCount);
        }
        else
        {
            _logger.LogWarning("Attempted to remove fish when count is already 0");
        }
        await NotifyClients();
        return _fishCount;
    }

    public int GetFishCount()
    {
        _logger.LogDebug("Fish count requested: {FishCount}", _fishCount);
        return _fishCount;
    }

    private async Task NotifyClients()
    {
        _logger.LogDebug("Notifying clients of updated fish count: {FishCount}", _fishCount);
        await _hubContext.Clients.All.SendAsync("ReceiveFishCount", _fishCount);
    }
}