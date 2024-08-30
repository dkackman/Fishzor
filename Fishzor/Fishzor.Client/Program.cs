using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fishzor.Client.State;

Console.WriteLine("Client Program.cs is starting");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

Console.WriteLine("WebAssemblyHostBuilder created");
string baseAddress;
if (builder.HostEnvironment.IsDevelopment())
{
    baseAddress = "https://localhost:7233/"; // Replace with your actual development server port
}
else
{
    baseAddress = builder.HostEnvironment.BaseAddress;
}

builder.Services.AddScoped<FishTankState>();

var host = builder.Build();

var fishTankState = host.Services.GetRequiredService<FishTankState>();
await fishTankState.InitializeAsync($"{baseAddress}fishhub");

await host.RunAsync();

Console.WriteLine("Client application started");