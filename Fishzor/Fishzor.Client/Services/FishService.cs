using Microsoft.AspNetCore.Components;
using Fishzor.Client.Components;

namespace Fishzor.Client.Services;

public class FishService
{
    public event Action? OnChange;
    private List<RenderFragment> _fishComponents = new();

    public IReadOnlyList<RenderFragment> FishComponents => _fishComponents.AsReadOnly();

    public void AddFish()
    {
        _fishComponents.Add(new RenderFragment(builder =>
        {
            builder.OpenComponent<Fish>(0);
            builder.CloseComponent();
        }));
        NotifyStateChanged();
    }

    public void RemoveFish()
    {
        if (_fishComponents.Count > 0)
        {
            _fishComponents.RemoveAt(_fishComponents.Count - 1);
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
