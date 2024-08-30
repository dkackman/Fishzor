using Fishzor.Client.Pages;
using Fishzor.Components;
using Fishzor.Services;
using Fishzor.Client.State;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fishzor.Hubs;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();
builder.Services.AddSingleton<FishService>();
builder.Services.AddScoped<FishTankState>();
builder.Services.AddSignalR();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application starting up");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    logger.LogInformation("Development environment detected");
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    logger.LogInformation("Production environment detected");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

logger.LogDebug("Configuring Razor Components");
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Fishzor.Client._Imports).Assembly);

logger.LogDebug("Mapping SignalR hub");
app.MapHub<FishHub>("/fishhub");

logger.LogInformation("Application configured, starting to run");
app.Run();
logger.LogInformation("Application has stopped");