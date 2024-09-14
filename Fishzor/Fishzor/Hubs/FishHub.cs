using Microsoft.AspNetCore.SignalR;
using Fishzor.Services;
using Fishzor.Client.State;
using Fishzor.Client.Components;
using Ganss.Xss;

namespace Fishzor.Hubs;

public class FishHub(FishService fishService, HtmlSanitizer sanitizer, ILogger<FishHub> logger) : Hub
{
    private readonly FishService _fishService = fishService;
    private readonly HtmlSanitizer _sanitizer = sanitizer;

    private readonly ILogger<FishHub> _logger = logger;

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("New client connected: {ConnectionId}", Context.ConnectionId);
        _fishService.AddFish(Context.ConnectionId);
        await Clients.All.SendAsync("ReceiveFishState", _fishService.ConnectedFish);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        _fishService.RemoveFish(Context.ConnectionId);
        await Clients.All.SendAsync("ReceiveFishState", _fishService.ConnectedFish);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task BroadcastMessage(ChatMessage message)
    {
        if (_fishService.Fish.TryGetValue(Context.ConnectionId, out var fishState))
        {
            var fishMessage = new FishMessage()
            {
                ClientId = Context.ConnectionId,
                Message = new ChatMessage
                {
                    Message = _sanitizer.Sanitize(message.Message),
                    Modifier = _sanitizer.Sanitize(message.Modifier)
                },
                Color = fishState.Color,
            };
            await Clients.All.SendAsync("ReceiveMessage", fishMessage);
        }
        else
        {
            _logger.LogWarning("Received message to send but source client {ConnectionId} not found", Context.ConnectionId);
        }
    }
}