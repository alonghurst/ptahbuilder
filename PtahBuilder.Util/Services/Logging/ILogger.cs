namespace PtahBuilder.Util.Services.Logging;

public interface ILogger
{
    void Info(string message);
    void Warning(string message);
    void Error(string message);
    void Success(string message);
}