namespace Fishzor.Client.Components;

public class FishAnimation
{
    private static readonly Random _random = new();
    public Size Size { get; private set; } = new();
    public Point Position { get; private set; } = new();
    public Velocity Velocity { get; private set; } = GetRandomVelocity(_random.Next(0, 2) == 0 ? Direction.Left : Direction.Right);
    public bool Enabled { get; set; } = true;

    // this is called when dragging the fish is complete
    public void MoveFish(Point newPosition)
    {
        Position = newPosition;
        Velocity = GetRandomVelocity(Velocity.OtherDirection().Direction);
    }

    // this is called on the animation loop
    public void IncrementPosition(Velocity velocity, ClientRect tank, ClientRect fish)
    {
        Velocity = velocity;
        Size = new Size(fish.Height, fish.Width);
        var currentPosition = new Point(fish.Left, fish.Top);
        var nextPosition = currentPosition + Velocity;

        AdjustVelocityForBoundaries(nextPosition, tank);

        Position += Velocity;
    }

    public static Velocity GetRandomVelocity(Direction direction) 
    {
        var directionModifier = direction == Direction.Right ? 1 : -1;
        return new((_random.NextDouble() * 3.0 + 0.5) * directionModifier, (_random.NextDouble() - 0.5) * 0.5);
    }

    private void AdjustVelocityForBoundaries(Point nextPosition, ClientRect tankRect)
    {
        if (nextPosition.Top <= tankRect.Top)
        {
            Velocity = new Velocity(Velocity.Dx, Math.Abs(Velocity.Dy));
        }
        else if (nextPosition.Top + Size.Height >= tankRect.Bottom)
        {
            Velocity = new Velocity(Velocity.Dx, -Math.Abs(Velocity.Dy));
        }

        if (nextPosition.Left <= tankRect.Left)
        {
            Velocity = new Velocity(Math.Abs(Velocity.Dx), Velocity.Dy);
        }
        else if (nextPosition.Left + Size.Width >= tankRect.Right)
        {
            Velocity = new Velocity(-Math.Abs(Velocity.Dx), Velocity.Dy);
        }
    }
}