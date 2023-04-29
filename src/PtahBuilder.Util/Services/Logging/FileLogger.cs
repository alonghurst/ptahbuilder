using Spectre.Console;

namespace PtahBuilder.Util.Services.Logging;

public class FileLogger : ILogger, IDisposable
{
    private StreamWriter? _file;

    static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

    public FileLogger(string logDir = "Logs")
    {
        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }
        var fileName = Path.Combine(logDir, $"log_{DateTime.Now:yyyy-M-d-HH-mm-ss}.txt");

        _file = new StreamWriter(fileName);
    }

    private void CloseFile()
    {
        if (_file != null)
        {
            _file.Close();
        }
    }

    public void Dispose()
    {
        CloseFile();
    }

    public void Info(string message)
    {
        WriteLine(message);
    }

    private void WriteLine(string message)
    {
        _semaphoreSlim.Wait();
        _file?.WriteLine(message);
        _semaphoreSlim.Release();
    }

    public void Warning(string message)
    {
        WriteLine(message.RemoveMarkup());
    }

    public void Error(string message)
    {
        WriteLine(message);
    }

    public void Success(string message)
    {
        WriteLine(message);
    }

    public void Verbose(string message)
    {
    }
}