namespace Fishzor.Client.Components;

public readonly struct Velocity(double dx, double dy)
{
    public double dx { get; } = dx;
    public double dy { get; } = dy;

    public override readonly string ToString() => $"[dx: {dx}, dy: {dy}]";
}