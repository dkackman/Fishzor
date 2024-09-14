using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fishzor.Client.State;
using Fishzor.Client.Services;
using Fishzor.Client;
using Microsoft.Extensions.Logging.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<FishTankState>();
builder.Services.AddScoped<MessageDispatcher>();

// Configure logging
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.Services.AddSingleton<ILoggerProvider, BrowserConsoleLoggerProvider>();
var isDev = builder.HostEnvironment.IsDevelopment();
//if (isDev)
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger<Program>();

var fishTankState = host.Services.GetRequiredService<FishTankState>();


string baseAddress = isDev ? "https://localhost:7233/" : builder.HostEnvironment.BaseAddress;
await fishTankState.InitializeAsync($"{baseAddress}fishhub");

await host.RunAsync();

logger.LogInformation("Client app started.");
