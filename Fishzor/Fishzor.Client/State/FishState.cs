using Fishzor.Client.Components;

namespace Fishzor.Client.State;

public class FishState
{
    public string Id { get; init; } = string.Empty;
    public FishColor Color { get; init; } = FishColor.Orange;
    public string Scale { get; init; } = "100.0";
    public string CurrentMessage { get; set; } = "";
    public bool IsMessageVisible { get; set; } = false;
    override public string ToString() => $"[Id: {Id}, Color: {Color}]";
}