using Microsoft.AspNetCore.Components;
using Fishzor.Client.Components;
using System.Collections.Concurrent;

namespace Fishzor.Services;

public class FishService
{
    private readonly ConcurrentBag<RenderFragment> _fishComponents = new();

    public IReadOnlyCollection<RenderFragment> FishComponents => _fishComponents;

    public void AddFish()
    {
        _fishComponents.Add(new RenderFragment(builder =>
        {
            builder.OpenComponent<Fish>(0);
            builder.CloseComponent();
        }));
    }

    public void RemoveFish()
    {
        if (_fishComponents.TryTake(out _))
        {
            // Fish removed successfully
        }
    }
}