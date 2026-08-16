using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Plugins.GraphViz.Config;
using Rubjerg.Graphviz;

namespace PtahBuilder.Plugins.GraphViz.Steps;

public abstract class RenderGraphStep<T> : IStep<T>
{
    private readonly GraphSettings _graphSettings;
    private readonly string _filename;
    private readonly string? _outputDirectory;

    protected RenderGraphStep(GraphSettings graphSettings, string filename, string? outputDirectory = null)
    {
        _graphSettings = graphSettings;
        _filename = filename;
        _outputDirectory = outputDirectory;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var graph = RootGraph.CreateNew(GraphType, GraphName);

        try
        {
            await Render(context, entities, graph);

            var outputDirectory = OutputDirectory;

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var path = Path.Combine(outputDirectory, _filename);

            WriteGraph(graph, path);
        }
        finally
        {
            graph.Close();
        }
    }

    protected virtual string OutputDirectory =>
        string.IsNullOrWhiteSpace(_outputDirectory)
            ? _graphSettings.OutputDirectory
            : _outputDirectory!;

    protected virtual GraphType GraphType => _graphSettings.GraphType;

    protected virtual string GraphName => Path.GetFileNameWithoutExtension(_filename);

    protected virtual string LayoutEngine => _graphSettings.LayoutEngine;

    protected abstract Task Render(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities, RootGraph graph);

    private void WriteGraph(RootGraph graph, string path)
    {
        var extension = Path.GetExtension(path);

        if (extension.Equals(".dot", StringComparison.OrdinalIgnoreCase))
        {
            graph.ToDotFile(path);
        }
        else if (extension.Equals(".xdot", StringComparison.OrdinalIgnoreCase))
        {
            graph.ToXDotFile(path, LayoutEngine);
        }
        else if (extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
        {
            graph.ToPngFile(path, LayoutEngine);
        }
        else if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            graph.ToPdfFile(path, LayoutEngine);
        }
        else if (extension.Equals(".ps", StringComparison.OrdinalIgnoreCase))
        {
            graph.ToPsFile(path, LayoutEngine);
        }
        else
        {
            graph.ToSvgFile(path, LayoutEngine);
        }
    }
}
