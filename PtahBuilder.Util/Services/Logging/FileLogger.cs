using Spectre.Console;

namespace PtahBuilder.Util.Services.Logging;

public class FileLogger : ILogger, IDisposable
{
    private StreamWriter? _file;

    public FileLogger()
    {
        var fileName = $"log_{DateTime.Now:yyyy-M-d-HH-mm-ss}.txt";

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
        _file?.WriteLine(message);
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
}