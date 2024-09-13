using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fishzor.Client.State;
using Fishzor.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<FishTankState>();
builder.Services.AddScoped<MessageDispatcher>();
var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger<Program>();

var fishTankState = host.Services.GetRequiredService<FishTankState>();

string baseAddress = builder.HostEnvironment.IsDevelopment() ? "https://localhost:7233/" : builder.HostEnvironment.BaseAddress;
await fishTankState.InitializeAsync($"{baseAddress}fishhub");

await host.RunAsync();

logger.LogInformation("Client app started.");
