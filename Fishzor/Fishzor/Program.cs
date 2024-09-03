using Fishzor.Components;
using Fishzor.Services;
using Fishzor.Client.State;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fishzor.Hubs;
using Microsoft.Net.Http.Headers;

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
    app.UseHsts();
    logger.LogInformation("Production environment detected");
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var path = ctx.File.PhysicalPath ?? "";
        if (path.EndsWith(".jpg") || path.EndsWith(".png") || path.EndsWith(".gif"))
        {
            // Cache images for 7 days
            ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=604800";
        }
        else if (!path.Contains("_framework")) // Exclude WebAssembly files
        {
            // Cache other static files for 1 day
            ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=86400";
        }
        else
        {
            // Do not cache Blazor WebAssembly files
            ctx.Context.Response.Headers[HeaderNames.CacheControl] = "no-cache";
        }
    }
});

app.UseRouting();
app.UseAntiforgery();

logger.LogDebug("Configuring Razor Components");
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Fishzor.Client._Imports).Assembly);

logger.LogDebug("Mapping SignalR hub");
app.MapHub<FishHub>("/fishhub");

app.MapControllers();

logger.LogInformation("Application configured, starting to run");
app.Run();
logger.LogInformation("Application has stopped");