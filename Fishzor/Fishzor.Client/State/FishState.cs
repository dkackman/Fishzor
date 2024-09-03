using Fishzor.Client.Components;

namespace Fishzor.Client.State;

public class FishState
{
    public string Id { get; init; } = string.Empty;
    public FishColor Color { get; init; } = FishColor.Orange;
    public string Scale { get; init; } = "100.0";
    override public string ToString() => $"[Id: {Id}, Color: {Color}]";
}