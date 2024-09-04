using Fishzor;

var app = WebApplication.CreateBuilder(args)
    .ConfigureLogging()
    .ConfigureServices()
    .Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Configuring Application");
app.ConfigureMiddleware(logger)
    .ConfigureEndpoints();

logger.LogInformation("Application configured, starting to run");
app.Run();
logger.LogInformation("Application has stopped");