using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fishzor.Client.State;
using Fishzor.Client.Services;
using Microsoft.Extensions.Logging.Configuration;
using Ganss.Xss;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<FishTankState>();
builder.Services.AddScoped<MessageDispatcher>();
builder.Services.AddSingleton<HtmlSanitizer>();

// Configure logging
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.SetMinimumLevel(LogLevel.Trace);
var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger<Program>();

var fishTankState = host.Services.GetRequiredService<FishTankState>();

var baseAddress = builder.HostEnvironment.IsDevelopment() ? "https://localhost:7233/" : builder.HostEnvironment.BaseAddress;
await fishTankState.InitializeAsync($"{baseAddress}fishhub");

await host.RunAsync();

logger.LogInformation("Client app started.");
