﻿using System.Drawing;

#pragma warning disable CA1416 // Validate platform compatibility
namespace PtahBuilder.Plugins.Imaging.Steps;

public static class ImagingDebug
{
    public static Font Font { get; } = new(FontFamily.GenericMonospace, 10);
}
#pragma warning restore CA1416 // Validate platform compatibility