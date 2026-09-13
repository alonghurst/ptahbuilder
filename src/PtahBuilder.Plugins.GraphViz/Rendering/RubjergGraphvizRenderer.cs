using Rubjerg.Graphviz;
using RubjergRootGraph = Rubjerg.Graphviz.RootGraph;

namespace PtahBuilder.Plugins.GraphViz.Rendering;

public class RubjergGraphvizRenderer : IGraphvizRenderer
{
    public string Name => "Rubjerg.Graphviz";

    public bool IsAvailable() => OperatingSystem.IsWindows();

    public void RenderDot(string dotPath, string outputPath, string layoutEngine)
    {
        if (!IsAvailable())
        {
            throw new InvalidOperationException($"{Name} is not supported on this platform.");
        }

        Require.NotNullOrWhiteSpace(dotPath, nameof(dotPath));
        Require.NotNullOrWhiteSpace(outputPath, nameof(outputPath));

        var graph = RubjergRootGraph.FromDotFile(dotPath);
        try
        {
            var engine = string.IsNullOrWhiteSpace(layoutEngine) ? LayoutEngines.Dot : layoutEngine;
            graph.ToPngFile(outputPath, engine);
        }
        finally
        {
            graph.Close();
        }
    }
}
