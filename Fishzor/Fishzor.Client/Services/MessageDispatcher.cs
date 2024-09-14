using Fishzor.Client.State;
using Fishzor.Client.Components;

namespace Fishzor.Client.Services;

public class MessageDispatcher(FishTankState fishTankState, ILogger<MessageDispatcher> logger)
{
    private readonly FishTankState _fishTankState = fishTankState;
    private readonly ILogger<MessageDispatcher> _logger = logger;

    public async Task DispatchMessageAsync(string message)
    {
        var cleanedMessage = message.Trim();
        if (!string.IsNullOrWhiteSpace(cleanedMessage))
        {
            if (cleanedMessage.StartsWith('/'))
            {
                ProcessCommand(cleanedMessage);
            }
            else
            {
                await _fishTankState.SendMessageAsync(cleanedMessage);
            }
        }
    }

    public event Action<ToastMessage>? OnFloatingMessageRequested;
    public event Action<string>? OnOpenUrlRequested;

    private void ProcessCommand(string command)
    {
        var parts = command.Split(' ');
        var commandName = parts[0].ToLower();
        _logger.LogInformation("Processing command: {commandName}", commandName);
        switch (commandName)
        {
            case "/help":
                DisplayHelpMessage();
                break;
            case "/about":
                OnOpenUrlRequested?.Invoke("https://github.com/dkackman/Fishzor");
                break;
            // Add more commands here as needed
            default:
                _logger.LogDebug("Unknown command: {commandName}", commandName);
                break;
        }
    }

    private void DisplayHelpMessage()
    {
        IEnumerable<string> helpMessages = [
            "/about - Show the about page",
            "/help - Display this help message"
            ];
        OnFloatingMessageRequested?.Invoke(new ToastMessage
        {
            Title = "Help",
            Caption = "Available commands",
            Messages = helpMessages,
        });
    }
}