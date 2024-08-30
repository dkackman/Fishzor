using Microsoft.AspNetCore.SignalR;
using Fishzor.Hubs;

namespace Fishzor.Services;

public class FishService
{
    private int _fishCount = 0;
    private readonly IHubContext<FishHub> _hubContext;

    public FishService(IHubContext<FishHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task<int> AddFish()
    {
        _fishCount++;
        await NotifyClients();
        return _fishCount;
    }

    public async Task<int> RemoveFish()
    {
        if (_fishCount > 0)
        {
            _fishCount--;
        }
        await NotifyClients();
        return _fishCount;
    }

    public int GetFishCount()
    {
        return _fishCount;
    }

    private async Task NotifyClients()
    {
        await _hubContext.Clients.All.SendAsync("ReceiveFishCount", _fishCount);
    }
}