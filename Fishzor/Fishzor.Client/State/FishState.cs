using Fishzor.Client.Components;

namespace Fishzor.Client.State;

public class FishState
{
    public string Id { get; set; } = string.Empty;
    public FishColor Color { get; set; } = FishColor.Orange;
}