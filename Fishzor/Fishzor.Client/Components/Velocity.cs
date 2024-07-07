namespace Fishzor.Client.Components;

public struct Velocity(double dx, double dy)
{
    public double dx { get; set; } = dx;
    public double dy { get; set; } = dy;

    public override readonly string ToString() => $"[dx: {dx}, dy: {dy}]";
}