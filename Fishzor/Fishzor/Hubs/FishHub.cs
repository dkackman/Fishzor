using Microsoft.AspNetCore.SignalR;
using Fishzor.Services;

namespace Fishzor.Hubs;

public class FishHub : Hub
{
    private readonly FishService _fishService;

    public FishHub(FishService fishService)
    {
        _fishService = fishService;
    }

    public async Task AddFish()
    {
        int newCount = await _fishService.AddFish();
        await Clients.All.SendAsync("ReceiveFishCount", newCount);
    }

    public async Task RemoveFish()
    {
        int newCount = await _fishService.RemoveFish();
        await Clients.All.SendAsync("ReceiveFishCount", newCount);
    }

    public async Task GetFishCount()
    {
        int count = _fishService.GetFishCount();
        await Clients.Caller.SendAsync("ReceiveFishCount", count);
    }

    public override async Task OnConnectedAsync()
    {
        await GetFishCount();
        await base.OnConnectedAsync();
    }
}