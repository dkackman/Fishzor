using Fishzor.Client.Services;
using Fishzor.Client;
using Ganss.Xss;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services
    .AddScoped<FishTankClient>()
    .AddScoped<MessageDispatcher>()
    .AddSingleton<HtmlSanitizer>()
    .AddSingleton<Animator>();

// Configure logging
builder.Logging
    .AddConfiguration(builder.Configuration.GetSection("Logging"))
    .SetMinimumLevel(LogLevel.Warning);

var host = builder.Build();

var logger = host.Services
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger<Program>();

var baseAddress = builder.HostEnvironment.IsDevelopment() ? "https://localhost:7233/" : builder.HostEnvironment.BaseAddress;
var fishTankClient = host.Services.GetRequiredService<FishTankClient>();

await fishTankClient.InitializeAsync($"{baseAddress}fishhub");

await host.RunAsync();

logger.LogInformation("Client app started.");
