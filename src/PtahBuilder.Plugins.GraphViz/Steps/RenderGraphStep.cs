using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Plugins.GraphViz.Config;
using PtahBuilder.Plugins.GraphViz.Rendering;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.Plugins.GraphViz.Steps;

public abstract class RenderGraphStep<T> : IStep<T>
{
    private readonly GraphSettings _graphSettings;
    private readonly IGraphvizRendererResolver _rendererResolver;
    private readonly ILogger _logger;
    private readonly string _filename;
    private readonly string? _outputDirectory;

    protected RenderGraphStep(
        GraphSettings graphSettings,
        IGraphvizRendererResolver rendererResolver,
        ILogger logger,
        string filename,
        string? outputDirectory = null)
    {
        _graphSettings = graphSettings;
        _rendererResolver = rendererResolver;
        _logger = logger;
        _filename = filename;
        _outputDirectory = outputDirectory;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var graph = new RootGraph(GraphName, Directed);
        await Render(context, entities, graph);

        var outputDirectory = OutputDirectory;
        Directory.CreateDirectory(outputDirectory);

        var basename = Path.GetFileNameWithoutExtension(_filename);
        var dotPath = Path.Combine(outputDirectory, $"{basename}.dot");
        graph.WriteDotFile(dotPath);

        TryRenderImage(dotPath, basename, outputDirectory);
    }

    protected virtual string OutputDirectory =>
        string.IsNullOrWhiteSpace(_outputDirectory)
            ? _graphSettings.OutputDirectory
            : _outputDirectory!;

    protected virtual bool Directed => _graphSettings.Directed;

    protected virtual string GraphName => Path.GetFileNameWithoutExtension(_filename);

    protected virtual string LayoutEngine => _graphSettings.LayoutEngine;

    protected abstract Task Render(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities, RootGraph graph);

    private void TryRenderImage(string dotPath, string basename, string outputDirectory)
    {
        var format = NormalizeFormat(_graphSettings.RenderImageFormat);
        if (string.IsNullOrWhiteSpace(format))
        {
            _logger.Verbose($"Skipping image render for '{basename}' because no image format is configured.");
            return;
        }

        var renderer = _rendererResolver.Resolve();
        if (renderer == null)
        {
            _logger.Info($"Wrote '{dotPath}'. Skipping {format} render because no Graphviz wrapper is available on this platform.");
            return;
        }

        var imagePath = Path.Combine(outputDirectory, $"{basename}.{format}");

        try
        {
            renderer.RenderDot(dotPath, imagePath, LayoutEngine);
            _logger.Success($"Rendered '{imagePath}' using {renderer.Name}.");
        }
        catch (Exception exception)
        {
            _logger.Warning($"Wrote '{dotPath}' but failed to render '{imagePath}' using {renderer.Name}: {exception.Message}");
        }
    }

    private static string NormalizeFormat(string format)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            return string.Empty;
        }

        return format.Trim().TrimStart('.').ToLowerInvariant();
    }
}
