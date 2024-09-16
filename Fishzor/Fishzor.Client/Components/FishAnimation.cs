namespace Fishzor.Client.Components;

public class FishAnimation
{
    private readonly Random _random = new();
    public Size Size { get; private set; } = new();
    public Point Position { get; private set; } = new();
    public Velocity Velocity { get; private set; }
    public bool Enabled { get; set; } = true;

    public FishAnimation()
    {
        Velocity = GetRandomVelocity(_random.Next(0, 2) == 0 ? Direction.Left : Direction.Right);
    }

    public void InitializePosition(ClientRect tankRect)
    {
        // the 50 is buffer to keep the fish from being placed too close to the edge
        var left = (tankRect.Width - 50.0) * _random.NextDouble();
        var top = (tankRect.Height - 50.0) * _random.NextDouble();

        MoveFish(new Point(left, top));
    }

    // this is called when dragging the fish is complete
    public void MoveFish(Point newPosition)
    {
        Position = newPosition;
        Velocity = GetRandomVelocity(Velocity.OtherDirection().Direction);
    }

    // this is called on the animation loop
    public void IncrementPosition(ClientRect tank, ClientRect fish)
    {
        var nextVelocity = GetNextVelocity();
        Size = new Size(fish.Height, fish.Width);

        var currentPosition = new Point(fish.Left, fish.Top);
        var nextPosition = currentPosition + Velocity;

        Velocity = AdjustVelocityForBoundaries(nextVelocity, Size, nextPosition, tank);
        Position += Velocity;
    }

    public Velocity GetRandomVelocity(Direction direction)
    {
        var directionModifier = direction == Direction.Right ? 1 : -1;
        return new((_random.NextDouble() * 3.0 + 0.5) * directionModifier, (_random.NextDouble() - 0.5) * 0.5);
    }

    private Velocity GetNextVelocity()
    {
        // randomly change direction and/or speed
        var changeDirection = _random.Next(0, 400) < 1;
        var changeSpeed = _random.Next(0, 300) < 1;
        var newDirectionVelocity = changeDirection ? Velocity.OtherDirection() : Velocity;

        // always change speed if direction changed
        return changeSpeed || changeDirection ? GetRandomVelocity(newDirectionVelocity.Direction) : newDirectionVelocity;
    }

    private static Velocity AdjustVelocityForBoundaries(Velocity currentVelocity, Size size, Point nextPosition, ClientRect tankRect)
    {
        // check the y bounds
        if (nextPosition.Top <= tankRect.Top)
        {
            currentVelocity = new Velocity(currentVelocity.Dx, Math.Abs(currentVelocity.Dy));
        }
        else if (nextPosition.Top + size.Height >= tankRect.Bottom)
        {
            currentVelocity = new Velocity(currentVelocity.Dx, -Math.Abs(currentVelocity.Dy));
        }

        // check the x bounds
        if (nextPosition.Left <= tankRect.Left)
        {
            currentVelocity = new Velocity(Math.Abs(currentVelocity.Dx), currentVelocity.Dy);
        }
        else if (nextPosition.Left + size.Width >= tankRect.Right)
        {
            currentVelocity = new Velocity(-Math.Abs(currentVelocity.Dx), currentVelocity.Dy);
        }

        return currentVelocity;
    }
}