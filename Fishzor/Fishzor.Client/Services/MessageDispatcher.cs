using Fishzor.Client.State;

namespace Fishzor.Client.Services;

public class MessageDispatcher(FishTankState fishTankState)
{
    private readonly FishTankState _fishTankState = fishTankState;

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

    private void ProcessCommand(string command)
    {
        var parts = command.Split(' ');
        var commandName = parts[0].ToLower();

        switch (commandName)
        {
            case "/help":
                DisplayHelpMessage();
                break;
            // Add more commands here as needed
            default:
                Console.WriteLine($"Unknown command: {commandName}");
                break;
        }
    }

    private void DisplayHelpMessage()
    {
        var helpMessage = @"Available commands:
/help - Display this help message
// Add more commands to this list as you implement them
";
        Console.WriteLine(helpMessage);
        // You might want to return this message or use an event to display it in the UI
    }
}