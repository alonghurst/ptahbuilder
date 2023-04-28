using System.Drawing;
using System.Text;

#pragma warning disable CA1416 // Validate platform compatibility
namespace PtahBuilder.BuildSystem.Extensions;

public static class GraphicsExtensions
{
    public static PointF DrawString(this Graphics graphics, string text, Font font, Brush brush, PointF location, float maxWidth)
    {
        string[] words = text.Split(' ');

        StringBuilder currentLine = new StringBuilder();
        SizeF size;

        foreach (string word in words)
        {
            currentLine.Append(word + " ");
            size = graphics.MeasureString(currentLine.ToString(), font);

            if (size.Width > maxWidth)
            {
                // The current line is too long, so draw it and start a new one.
                graphics.DrawString(currentLine.ToString().TrimEnd(), font, brush, location);
                location.Y += font.Height;
                currentLine = new StringBuilder(word + " ");
            }
        }

        // Draw the last line.
        graphics.DrawString(currentLine.ToString().TrimEnd(), font, brush, location);

        location.Y += font.Height;

        return location;
    }
}

#pragma warning restore CA1416 // Validate platform compatibility