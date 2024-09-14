using Fishzor.Client.State;

namespace Fishzor.Client.Services;

public class MessageDispatcher(FishTankState fishTankState, ILogger<MessageDispatcher> logger)
{
    private readonly FishTankState _fishTankState = fishTankState;
    private readonly ILogger<MessageDispatcher> _logger = logger;

    public async Task DispatchMessageAsync(string message)
    {
        var cleanedMessage = message.Trim();
        if (string.IsNullOrWhiteSpace(cleanedMessage))
        {
            return;
        }

        if (cleanedMessage.StartsWith('/'))
        {
            ProcessCommand(cleanedMessage);
        }
        else
        {
            await _fishTankState.SendMessageAsync(cleanedMessage);
        }
    }

    public event Action<string>? OnHelpRequested;

    private void ProcessCommand(string command)
    {
        var parts = command.Split(' ');
        var commandName = parts[0].ToLower();
        _logger.LogDebug("Processing command: {commandName}", commandName);
        switch (commandName)
        {
            case "/help":
                DisplayHelpMessage();
                break;
            // Add more commands here as needed
            default:
                _logger.LogDebug("Unknown command: {commandName}", commandName);
                break;
        }
    }

    private void DisplayHelpMessage()
    {
        var helpMessage = @"Available commands:
/help - Display this help message
// Add more commands to this list as you implement them
";
        OnHelpRequested?.Invoke(helpMessage);
        // You might want to return this message or use an event to display it in the UI
    }
}