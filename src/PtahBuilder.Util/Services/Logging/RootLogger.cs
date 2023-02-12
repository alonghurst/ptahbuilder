namespace PtahBuilder.Util.Services.Logging;

public class RootLogger : ILogger, IDisposable
{
    private readonly ConsoleLogger _consoleLogger;
    private readonly FileLogger _fileLogger;
    private readonly ILogger[] _loggers;

    public RootLogger()
    {
        _fileLogger = new FileLogger();
        _consoleLogger = new ConsoleLogger();

        _loggers = new ILogger[] { _fileLogger, _consoleLogger };
    }

    public void Dispose()
    {
        _fileLogger.Dispose();
    }

    public void Info(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.Info(message);
        }
    }

    public void Warning(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.Warning(message);
        }
    }

    public void Error(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.Error(message);
        }
    }

    public void Success(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.Success(message);
        }
    }
}