namespace Fishzor.Client.Components;

public class ChatMessage
{
    public string Message { get; init; } = "";
    public string Modifier { get; init; } = "";

    public bool IsCommand => Commands.ContainsKey(Modifier);
    public bool IsEmpty => string.IsNullOrWhiteSpace(Message) && string.IsNullOrWhiteSpace(Modifier);

    // Override Equals method
    public override bool Equals(object? obj) => Equals(obj as ChatMessage);

    // Implement IEquatable<ChatMessage>
    public bool Equals(ChatMessage? other)
    {
        if (other == null)
            return false;

        return Message == other.Message && Modifier == other.Modifier;
    }

    // Override GetHashCode method
    public override int GetHashCode() => HashCode.Combine(Message, Modifier);

    // Equality operator
    public static bool operator ==(ChatMessage? left, ChatMessage? right)
    {
        if (left is null)
            return right is null;

        return left.Equals(right);
    }

    // Inequality operator
    public static bool operator !=(ChatMessage? left, ChatMessage? right) => !(left == right);

    public static readonly IReadOnlyDictionary<string, string> Commands = new Dictionary<string, string>
    {
        {"about", "Show the about page"},
        {"help", "Display this help"},
    };

    public static readonly IReadOnlyDictionary<string, string> Emotes = new Dictionary<string, string>
    {
        {"shout", "Shout a chat message"},
        {"whisper", "Whisper a chat message"},
    };

    public static ChatMessage FromMessage(string message)
    {
        message = message.Trim();

        if (message.StartsWith('/'))
        {
            // Format 2: /command argument
            var parts = message[1..].Split(' ', 2);
            var modifier = parts[0].ToLower();
            var messageText = parts.Length > 1 ? parts[1] : "";
            return new ChatMessage
            {
                Message = messageText,
                Modifier = modifier,
            };
        }

        return new ChatMessage
        {
            Message = message,
            Modifier = "",
        };
    }
}