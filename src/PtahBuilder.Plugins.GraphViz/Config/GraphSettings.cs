using Rubjerg.Graphviz;

namespace PtahBuilder.Plugins.GraphViz.Config;

public class GraphSettings
{
    public string OutputDirectory { get; set; } = string.Empty;

    public GraphType GraphType { get; set; } = GraphType.Directed;

    public string LayoutEngine { get; set; } = LayoutEngines.Dot;
}
