using Fishzor.Client.Services;
using Fishzor.Client.State;
using Ganss.Xss;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services
    .AddScoped<FishTankState>()
    .AddScoped<MessageDispatcher>()
    .AddSingleton<HtmlSanitizer>()
    .AddSingleton<Animator>();

// Configure logging
builder.Logging
    .AddConfiguration(builder.Configuration.GetSection("Logging"))
    .SetMinimumLevel(LogLevel.Trace);

var host = builder.Build();

var logger = host.Services
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger<Program>();

var baseAddress = builder.HostEnvironment.IsDevelopment() ? "https://localhost:7233/" : builder.HostEnvironment.BaseAddress;
var fishTankState = host.Services.GetRequiredService<FishTankState>();

await fishTankState.InitializeAsync($"{baseAddress}fishhub");

await host.RunAsync();

logger.LogInformation("Client app started.");
