namespace PtahBuilder.Util.Services.Logging;

public class ConsoleLogger : ILogger
{
    public void Info(string message)
    {
        Console.WriteLine(message);
    }

    public void Warning(string message)
    {
        LogWithColour(ConsoleColor.Yellow, message);
    }

    public void Error(string message)
    {
        LogWithColour(ConsoleColor.Red, message);
    }

    public void Success(string message)
    {
        LogWithColour(ConsoleColor.Green, message);
    }

    private void LogWithColour(ConsoleColor colour, object message)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = colour;

        Console.WriteLine(message);

        Console.ForegroundColor = prev;
    }
}