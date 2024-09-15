using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Fishzor.Client.Components;

public class FishAnimator
{
    private readonly Random _random = new Random();
    private readonly IJSRuntime _jsRuntime;
    private readonly ElementReference _fishElement;
    private readonly bool _isClientFish;

    public Point Position { get; private set; }
    public Velocity FishVelocity { get; private set; }
    public bool Animate { get; set; } = true;

    public FishAnimator(IJSRuntime jsRuntime, ElementReference fishElement, bool isClientFish)
    {
        _jsRuntime = jsRuntime;
        _fishElement = fishElement;
        _isClientFish = isClientFish;
        InitializeVelocity();
    }

    public async Task InitializePositionAsync()
    {
        var tankRect = await _jsRuntime.InvokeAsync<ClientRect>("getTankRect");
        var left = (tankRect.Width - 50.0) * _random.NextDouble();
        var top = (tankRect.Height - 50.0) * _random.NextDouble();
        Position = new Point(left, top);
    }

    private void InitializeVelocity()
    {
        var direction = _random.Next(0, 2) == 0 ? -1 : 1;
        FishVelocity = new Velocity((_random.NextDouble() * 3.0 + 0.5) * direction, (_random.NextDouble() - 0.5) * 0.5);
    }

    public void NewVelocity()
    {
        var direction = FishVelocity.dx > 0 ? -1 : 1;
        FishVelocity = new Velocity((_random.NextDouble() * 3.0 + 0.5) * direction, (_random.NextDouble() - 0.5) * 0.5);
    }

    public async Task MoveFishAsync()
    {
        if (Animate)
        {
            if (_random.Next(0, 400) < 1)
            {
                NewVelocity();
            }

            var tankRect = await _jsRuntime.InvokeAsync<ClientRect>("getTankRect");
            var fishRect = await _jsRuntime.InvokeAsync<ClientRect>("getElementRect", _fishElement);
            var currentPosition = new Point(fishRect.Left, fishRect.Top);
            var nextPosition = currentPosition + FishVelocity;

            AdjustVelocityForBoundaries(nextPosition, tankRect);

            Position += FishVelocity;
        }
    }

    private void AdjustVelocityForBoundaries(Point nextPosition, ClientRect tankRect)
    {
        const int HEIGHT = 84;
        const int WIDTH = 144;

        if (nextPosition.Top <= tankRect.Top)
        {
            FishVelocity = new Velocity(FishVelocity.dx, Math.Abs(FishVelocity.dy));
        }
        else if (nextPosition.Top + HEIGHT >= tankRect.Bottom)
        {
            FishVelocity = new Velocity(FishVelocity.dx, -Math.Abs(FishVelocity.dy));
        }

        if (nextPosition.Left <= tankRect.Left)
        {
            FishVelocity = new Velocity(Math.Abs(FishVelocity.dx), FishVelocity.dy);
        }
        else if (nextPosition.Left + WIDTH >= tankRect.Right)
        {
            FishVelocity = new Velocity(-Math.Abs(FishVelocity.dx), FishVelocity.dy);
        }
    }

    public void UpdatePosition(double left, double top)
    {
        Position = new Point(left, top);
        NewVelocity();
    }
}
