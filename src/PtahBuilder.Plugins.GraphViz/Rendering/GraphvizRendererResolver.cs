namespace PtahBuilder.Plugins.GraphViz.Rendering;

public class GraphvizRendererResolver : IGraphvizRendererResolver
{
    private readonly IReadOnlyList<IGraphvizRenderer> _renderers;

    public GraphvizRendererResolver(IEnumerable<IGraphvizRenderer> renderers)
    {
        ArgumentNullException.ThrowIfNull(renderers);
        _renderers = renderers.ToArray();
    }

    public IGraphvizRenderer? Resolve() => _renderers.FirstOrDefault(renderer => renderer.IsAvailable());
}
