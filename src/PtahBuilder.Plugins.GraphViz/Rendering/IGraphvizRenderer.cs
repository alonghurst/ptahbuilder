namespace PtahBuilder.Plugins.GraphViz.Rendering;

public interface IGraphvizRenderer
{
    string Name { get; }

    bool IsAvailable();

    void RenderDot(string dotPath, string outputPath, string layoutEngine);
}
