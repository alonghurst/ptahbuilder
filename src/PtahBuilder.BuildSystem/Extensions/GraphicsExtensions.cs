using System.Drawing;
using System.Text;

#pragma warning disable CA1416 // Validate platform compatibility
namespace PtahBuilder.BuildSystem.Extensions;

public static class GraphicsExtensions
{
    public static PointF DrawString(this Graphics graphics, string text, Font font, Brush brush, PointF location, float maxWidth)
    {
        string[] words = text.Split(' ');

        var currentLine = new StringBuilder();

        foreach (string word in words)
        {
            var size = graphics.MeasureString(currentLine + " " + word, font);

            if (size.Width > maxWidth)
            {
                graphics.DrawString(currentLine.ToString().TrimEnd(), font, brush, location);
                location.Y += font.Height;
                currentLine = new();
            }

            currentLine.Append(word + " ");
        }

        var final = currentLine.ToString().TrimEnd();

        if (!string.IsNullOrWhiteSpace(final))
        {
            graphics.DrawString(final, font, brush, location);

            location.Y += font.Height;
        }

        return location;
    }
}

#pragma warning restore CA1416 // Validate platform compatibility