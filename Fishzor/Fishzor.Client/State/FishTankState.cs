using System;

namespace Fishzor.Client.State;

public class FishTankState
{
    private int _fishCount;
    public int FishCount
    {
        get => _fishCount;
        set
        {
            if (_fishCount != value)
            {
                _fishCount = value;
                OnStateChanged?.Invoke();
            }
        }
    }

    public event Action? OnStateChanged;
}
