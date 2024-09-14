using Fishzor.Client.State;
using Fishzor.Client.Components;

namespace Fishzor.Client.Services;

public class MessageDispatcher(FishTankState fishTankState, ILogger<MessageDispatcher> logger)
{
    private readonly FishTankState _fishTankState = fishTankState;
    private readonly ILogger<MessageDispatcher> _logger = logger;

    public async Task DispatchMessageAsync(string message)
    {
        var chatMessage = ChatMessage.FromMessage(message);

        if (!chatMessage.IsEmpty)
        {
            if (chatMessage.IsCommand)
            {
                ProcessCommand(chatMessage.Modifier);
            }
            else
            {
                await _fishTankState.SendMessageAsync(chatMessage.Message);
            }
        }
    }

    public event Action<ToastMessage>? OnFloatingMessageRequested;
    public event Action<string>? OnOpenUrlRequested;

    private void ProcessCommand(string command)
    {
        _logger.LogInformation("Processing command: {commandName}", command);
        switch (command)
        {
            case "help":
                DisplayHelpMessage();
                break;
            case "about":
                OnOpenUrlRequested?.Invoke("https://github.com/dkackman/Fishzor");
                break;
            // Add more commands here as needed
            default:
                _logger.LogDebug("Unknown command: {commandName}", command);
                break;
        }
    }

    private void DisplayHelpMessage()
    {
        IEnumerable<string> helpMessages = [
            "/about - Show the about page",
            "/help - Display this help message",
            "/shout - Shout a chat message",
            ];
        OnFloatingMessageRequested?.Invoke(new ToastMessage
        {
            Title = "Help",
            Caption = "Available commands",
            Messages = helpMessages,
        });
    }
}