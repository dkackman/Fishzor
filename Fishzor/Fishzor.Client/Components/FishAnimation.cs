namespace Fishzor.Client.Components;

public class FishAnimation
{
    private static readonly Random _random = new();
    public Point Position { get; private set; } = new();
    public Velocity Velocity { get; private set; } = GetRandomVelocity(_random.Next(0, 2) == 0 ? -1 : 1);
    public Direction Direction => Velocity.dx > 0 ? Direction.Right : Direction.Left;
    public bool Enabled { get; set; } = true;

    public void MoveFish(Point newPosition)
    {
        Position = newPosition;
        Velocity = GetRandomVelocity(Velocity.dx > 0 ? -1 : 1);
    }

    public void IncrementPosition(Velocity velocity, ClientRect tank, ClientRect fish)
    {
        Velocity = velocity;
        var currentPosition = new Point(fish.Left, fish.Top);
        var nextPosition = currentPosition + Velocity;

        AdjustVelocityForBoundaries(nextPosition, tank, fish);

        Position += Velocity;
    }

    public static Velocity GetRandomVelocity(int direction) => new((_random.NextDouble() * 3.0 + 0.5) * direction, (_random.NextDouble() - 0.5) * 0.5);

    private void AdjustVelocityForBoundaries(Point nextPosition, ClientRect tankRect, ClientRect fish)
    {
        if (nextPosition.Top <= tankRect.Top)
        {
            Velocity = new Velocity(Velocity.dx, Math.Abs(Velocity.dy));
        }
        else if (nextPosition.Top + fish.Height >= tankRect.Bottom)
        {
            Velocity = new Velocity(Velocity.dx, -Math.Abs(Velocity.dy));
        }

        if (nextPosition.Left <= tankRect.Left)
        {
            Velocity = new Velocity(Math.Abs(Velocity.dx), Velocity.dy);
        }
        else if (nextPosition.Left + fish.Width >= tankRect.Right)
        {
            Velocity = new Velocity(-Math.Abs(Velocity.dx), Velocity.dy);
        }
    }
}