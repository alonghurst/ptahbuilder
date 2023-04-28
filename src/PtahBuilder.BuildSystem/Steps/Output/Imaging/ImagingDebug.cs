using System.Drawing;

#pragma warning disable CA1416 // Validate platform compatibility
namespace PtahBuilder.BuildSystem.Steps.Output.Imaging;

public static class ImagingDebug
{
    public static Font Font { get; } = new Font(FontFamily.GenericMonospace, 10);
}
#pragma warning restore CA1416 // Validate platform compatibility