using Fishzor.Components;
using Fishzor.Services;
using Fishzor.Client.State;
using Fishzor.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Net.Http.Headers;

namespace Fishzor;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        return builder;
    }

    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();
        builder.Services.AddControllers();
        builder.Services.AddSingleton<FishService>();
        builder.Services.AddScoped<FishTankState>();
        builder.Services.AddSignalR();
        builder.Services.AddResponseCompression(options =>
        {
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
        });
        return builder;
    }

    public static WebApplication ConfigureMiddleware(this WebApplication app, ILogger logger)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            logger.LogInformation("Development environment detected");
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            logger.LogInformation("Production environment detected");
        }

        app.UseHttpsRedirection();
        app.UseHsts();
        app.Use(async (context, next) =>
        {
            // Remove potentially dangerous headers
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Remove("Server");

            // Add security headers
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
                "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
                "img-src 'self' data:; " +
                "font-src 'self'; " +
                "connect-src 'self' wss:;"
            );
            await next();
        });

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                var path = ctx.File.PhysicalPath ?? "";
                string cacheControl;

                switch (Path.GetExtension(path).ToLower())
                {
                    case ".jpg":
                    case ".png":
                    case ".gif":
                        cacheControl = "public,max-age=604800"; // 7 days
                        break;
                    case ".js":
                    case ".css":
                        cacheControl = "public,max-age=86400"; // 1 day
                        break;
                    default:
                        cacheControl = path.Contains("_framework") ? "no-cache" : "public,max-age=3600"; // 1 hour for others
                        break;
                }

                ctx.Context.Response.Headers[HeaderNames.CacheControl] = cacheControl;
            }
        });

        app.UseRouting();
        app.UseAntiforgery();
        app.UseResponseCompression();

        return app;
    }

    public static WebApplication ConfigureEndpoints(this WebApplication app, ILogger logger)
    {
        logger.LogDebug("Configuring Razor Components");
        app.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Fishzor.Client._Imports).Assembly);

        logger.LogDebug("Mapping SignalR hub");
        app.MapHub<FishHub>("/fishhub");

        app.MapControllers();
        return app;
    }
}