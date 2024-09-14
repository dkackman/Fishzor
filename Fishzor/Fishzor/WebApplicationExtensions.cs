using Fishzor.Components;
using Fishzor.Services;
using Fishzor.Client.State;
using Fishzor.Client.Services;
using Fishzor.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;
using System.Threading.RateLimiting;
using System.Text;
using Ganss.Xss;

namespace Fishzor;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging
            .ClearProviders()
            .AddConfiguration(builder.Configuration.GetSection("Logging"))
            .AddConsole()
            .AddDebug();

        return builder;
    }

    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();
        builder.Services
            .AddControllers();
        builder.Services
            .AddSignalR();
        builder.Services
            .AddSingleton<FishService>()
            .AddSingleton<HtmlSanitizer>()
            .AddScoped<FishTankState>()
            .AddScoped<MessageDispatcher>()
            .AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
            });
        builder.Services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });
        builder.Services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });
        builder.Services.AddHealthChecks();

        return builder;
    }

    public static WebApplication ConfigureMiddleware(this WebApplication app, ILogger logger)
    {
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

        app.UseHttpsRedirection()
            .UseHsts()
            .Use(AddSecurityHeaders)
            .UseBlazorFrameworkFiles()
            .UseStaticFiles(new StaticFileOptions { OnPrepareResponse = SetCacheHeaders })
            .UseRouting()
            .UseAntiforgery()
            .UseResponseCompression()
            .UseRateLimiter();

        return app;
    }

    public static WebApplication ConfigureEndpoints(this WebApplication app)
    {
        app.MapRazorComponents<App>()
           .AddInteractiveWebAssemblyRenderMode()
           .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.MapHub<FishHub>("/fishhub");
        app.MapControllers();
        app.MapHealthChecks("/health");

        return app;
    }

    private static async Task AddSecurityHeaders(HttpContext context, Func<Task> next)
    {
        // Remove potentially dangerous headers
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("Server");

        // Add security headers
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        if (context.Response.ContentType?.Contains("text/html", StringComparison.InvariantCultureIgnoreCase) == true)
        {
            var csp = new StringBuilder()
                .Append("default-src 'self'; ")
                .Append("script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; ")
                .Append("style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; ")
                .Append("img-src 'self' data:; ")
                .Append("font-src 'self'; ")
                .Append("connect-src 'self' wss:; ")
                .Append("frame-ancestors 'none';") // This replaces X-Frame-Options: DENY
                .ToString();
            context.Response.Headers.Append("Content-Security-Policy", csp);
        }

        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
        context.Response.Headers.Append("Permissions-Policy", "geolocation=(), midi=(), sync-xhr=(), microphone=(), camera=(), magnetometer=(), gyroscope=(), fullscreen=(self), payment=()");

        await next();
    }

    private static void SetCacheHeaders(StaticFileResponseContext ctx)
    {
        var path = ctx.File.PhysicalPath ?? "";
        string cacheControl = Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".jpg" or ".png" or ".gif" or ".webp" or ".svg" => "public,max-age=31536000,immutable", // 1 year
            ".js" or ".css" => "public,max-age=31536000,immutable", // 1 year
            _ => path.Contains("_framework") ? "no-cache" : "public,max-age=31536000" // 1 year for others, except _framework files
        };

        ctx.Context.Response.Headers[HeaderNames.CacheControl] = cacheControl;
    }
}