using Fishzor.Client.Components;

namespace Fishzor.Client.State;

public class FishMessage
{
    public string ClientId { get; init; } = string.Empty;
    public FishColor Color { get; init; } = FishColor.Orange;
    public string Message { get; init; } = "";
    override public string ToString() => $"[Id: {ClientId}, Message: {Message}]";
}