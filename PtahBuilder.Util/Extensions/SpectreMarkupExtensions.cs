using System.Text;
using Spectre.Console;

namespace PtahBuilder.Util.Extensions;

public static class SpectreMarkupExtensions
{
    private static readonly ConsoleColor[] RainbowColours = new[]
    {
        ConsoleColor.Red,
        ConsoleColor.Green,
        ConsoleColor.Yellow,
        ConsoleColor.Cyan,
    };

    public static string Rainbow(this string text)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            var c = RainbowColours[i % RainbowColours.Length];

            sb.Append(text[i].ToString().Colour(c));
        }

        return sb.ToString();
    }
    
    public static string Markedup(this string str, string markup)
    {
        return $"[{markup}]{str}[/]";
    }

    public static string Colour(this string str, ConsoleColor colour)
    {
        var colStr = ColourToString(colour);

        return str.Markedup(colStr);
    }

    private static string ColourToString(ConsoleColor colour)
    {
        switch (colour)
        {
            case ConsoleColor.DarkRed:
                return "maroon";
            case ConsoleColor.DarkGreen:
                return "green";
            case ConsoleColor.DarkYellow:
                return "olive";
            case ConsoleColor.DarkBlue:
                return "navy";
            case ConsoleColor.DarkCyan:
                return "teal";
            case ConsoleColor.DarkMagenta:
                return "purple";
            case ConsoleColor.Gray:
                return "silver";
            case ConsoleColor.DarkGray:
                return "gray";
            case ConsoleColor.Green:
                return "lime";
            case ConsoleColor.Magenta:
                return "fuchsia";
            case ConsoleColor.Cyan:
                return "aqua";
        }

        return colour.ToString().ToLower();
    }

    public static string Greet()
    {
        var game = "Conquistadors of Avarice".Rainbow();
        return $"Welcome to {game}!";
    }

    public static string ProperNoun(this string str)
    {
        return str.Colour(ConsoleColor.Yellow);
    }

    public static string Highlight(this string str)
    {
        return str.Colour(ConsoleColor.Green);
    }

    public static string Escape(this string str)
    {
        return Markup.Escape(str);
    }
    
    public static string Quote(this string str)
    {
        return $"'{str}'";
    }

    public static string MarkupCommand(this string command)
    {
        return command.Escape().Colour(ConsoleColor.DarkRed);
    }
}