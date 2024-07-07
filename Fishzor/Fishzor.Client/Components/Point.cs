namespace Fishzor.Client.Components;

public readonly struct Point(double left, double top)
{
    public double Left { get; } = left;
    public double Top { get; } = top;

    public override readonly string ToString() => $"[Left: {Left}, Top: {Top}]";

    // Operator overload to add Velocity to a Point
    public static Point operator +(Point point, Velocity velocity)
    {
        return new Point(point.Left + velocity.dx, point.Top + velocity.dy);
    }
}