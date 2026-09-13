namespace PtahBuilder.Plugins.GraphViz.Rendering;

public interface IGraphvizRendererResolver
{
    IGraphvizRenderer? Resolve();
}
