using System.Timers;

namespace Fishzor.Client.Services;

public class Animator : IDisposable
{
    private readonly System.Timers.Timer _animationTimer = new(50) { AutoReset = true, Enabled = false };
    private bool disposedValue;

    public Animator()
    {
        _animationTimer.Start();
        _animationTimer.Elapsed += OnTimerElapsedAsync;
    }

    private async void OnTimerElapsedAsync(object? sender, ElapsedEventArgs e)
    {
        OnAnimationTick?.Invoke();
        await Task.CompletedTask;
    }

    public event Action? OnAnimationTick;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _animationTimer.Stop();
                _animationTimer.Elapsed -= OnTimerElapsedAsync;
                _animationTimer.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}