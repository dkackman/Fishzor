using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fishzor.Client.Services;

Console.WriteLine("Client Program.cs is starting");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

Console.WriteLine("WebAssemblyHostBuilder created");

builder.Services.AddScoped<FishService>();

Console.WriteLine("FishService registered");

await builder.Build().RunAsync();

Console.WriteLine("Client application started");