using PtahBuilder.Util.Extensions;
using Spectre.Console;

namespace PtahBuilder.Util.Services.Logging;

public class ConsoleLogger : ILogger
{
    public void Info(string message)
    {
        AnsiConsole.MarkupLine(message);
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

    private void LogWithColour(ConsoleColor colour, string message)
    {
        AnsiConsole.MarkupLine(message.Colour(colour));
    }
}