namespace Fishzor.Client.Components;

public class ToastMessage
{
    public string Title { get; init; } = "";
    public string Caption { get; init; } = "";
    public IEnumerable<string> Messages { get; init; } = [];
}