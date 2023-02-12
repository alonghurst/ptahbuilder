namespace PtahBuilder.Util.Services;

public interface IDiagnostics
{
    void Time(string operationDescription, Action operation);
    T Time<T>(string operationDescription, Func<T> operation);

    Task Time(string operationDescription, Func<Task> operation);
}