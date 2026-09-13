using PtahBuilder.Plugins.GraphViz.Rendering;

namespace PtahBuilder.Tests.GraphViz;

public sealed class GraphvizRendererResolverTests
{
    [Fact]
    public void Resolve_ReturnsFirstAvailableRenderer()
    {
        var unavailable = new FakeRenderer("first", available: false);
        var available = new FakeRenderer("second", available: true);
        var later = new FakeRenderer("third", available: true);
        var resolver = new GraphvizRendererResolver([unavailable, available, later]);

        Assert.Same(available, resolver.Resolve());
    }

    [Fact]
    public void Resolve_ReturnsNullWhenNoRendererIsAvailable()
    {
        var resolver = new GraphvizRendererResolver([new FakeRenderer("none", available: false)]);

        Assert.Null(resolver.Resolve());
    }

    [Fact]
    public void RubjergGraphvizRenderer_IsAvailableOnWindowsOnly()
    {
        var renderer = new RubjergGraphvizRenderer();

        Assert.Equal(OperatingSystem.IsWindows(), renderer.IsAvailable());
    }

    [Fact]
    public void CliGraphvizRenderer_IsAvailableWhenDotIsFound()
    {
        var available = new CliGraphvizRenderer(() => "dot");
        var missing = new CliGraphvizRenderer(() => null);

        Assert.True(available.IsAvailable());
        Assert.False(missing.IsAvailable());
    }

    private sealed class FakeRenderer : IGraphvizRenderer
    {
        public FakeRenderer(string name, bool available)
        {
            Name = name;
            Available = available;
        }

        public string Name { get; }

        public bool Available { get; }

        public bool IsAvailable() => Available;

        public void RenderDot(string dotPath, string outputPath, string layoutEngine)
        {
        }
    }
}
