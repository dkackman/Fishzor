using Fishzor;

var builder = WebApplication.CreateBuilder(args)
    .ConfigureLogging()
    .ConfigureServices();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Configuring Application");
app.ConfigureMiddleware(logger)
    .ConfigureEndpoints(logger);

logger.LogInformation("Application configured, starting to run");
app.Run();
logger.LogInformation("Application has stopped");