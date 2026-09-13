namespace PtahBuilder.Plugins.GraphViz.Config;

public class GraphSettings
{
    public string OutputDirectory { get; set; } = string.Empty;

    public bool Directed { get; set; } = true;

    public string LayoutEngine { get; set; } = "dot";

    public string RenderImageFormat { get; set; } = "png";
}
