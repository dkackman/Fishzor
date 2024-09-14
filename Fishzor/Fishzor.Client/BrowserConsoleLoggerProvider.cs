namespace Fishzor.Client;

public class BrowserConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new BrowserConsoleLogger(categoryName);
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

public class BrowserConsoleLogger(string categoryName) : ILogger
{
    private readonly string _categoryName = categoryName;

    IDisposable? ILogger.BeginScope<TState>(TState state) => default;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Console.WriteLine($"[{_categoryName}] {logLevel}: {formatter(state, exception)}");
    }
}