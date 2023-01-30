using System.Diagnostics;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.Util.Services;

public class Diagnostics : IDiagnostics
{
    private readonly ILogger _logger;

    private int _depth = -1;

    public Diagnostics(ILogger logger)
    {
        _logger = logger;
    }

    public void Time(string operationDescription, Action operation)
    {
        _depth++;
        var sw = new Stopwatch();
        sw.Start();

        operation.Invoke();

        sw.Stop();

        WriteResult(operationDescription, sw);
        _depth--;
    }

    public T Time<T>(string operationDescription, Func<T> operation)
    {
        _depth++;
        var sw = new Stopwatch();
        sw.Start();

        var result = operation.Invoke();

        sw.Stop();

        WriteResult(operationDescription, sw);
        _depth--;

        return result;
    }

    private void WriteResult(string operationDescription, Stopwatch sw)
    {
        var timeStr = sw.Elapsed.TotalSeconds >= 10 ? $"{sw.Elapsed.TotalSeconds:F} sec" : $"{sw.ElapsedMilliseconds} ms";
        var prefix = new string(' ', _depth);

        _logger.Info($"{prefix}{operationDescription}: {timeStr}");
    }
}