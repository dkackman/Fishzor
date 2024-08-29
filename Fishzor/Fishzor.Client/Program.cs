using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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
await builder.Build().RunAsync();

Console.WriteLine("Client application started");