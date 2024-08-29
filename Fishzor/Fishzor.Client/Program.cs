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


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
builder.Services.AddScoped<FishTankState>();
await builder.Build().RunAsync();

Console.WriteLine("Client application started");