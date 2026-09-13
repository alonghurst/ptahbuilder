using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Plugins.GraphViz;
using PtahBuilder.Plugins.GraphViz.Config;
using PtahBuilder.Plugins.GraphViz.Rendering;
using PtahBuilder.Plugins.GraphViz.Steps;
using PtahBuilder.Util.Services;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.Tests.GraphViz;

public sealed class RenderGraphStepTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "PtahBuilder-RenderGraphStepTests", Guid.NewGuid().ToString("N"));

    public RenderGraphStepTests()
    {
        Directory.CreateDirectory(_directory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, true);
        }
    }

    [Fact]
    public async Task Execute_AlwaysWritesDotAndSkipsImageWhenNoRendererIsAvailable()
    {
        var logger = new CapturingLogger();
        var step = CreateStep(new GraphvizRendererResolver(Array.Empty<IGraphvizRenderer>()), logger, "MinionRelations.dot");

        await step.Execute(CreateContext(), Array.Empty<Entity<string>>());

        var dotPath = Path.Combine(_directory, "MinionRelations.dot");
        Assert.True(File.Exists(dotPath));
        Assert.Contains("digraph MinionRelations", File.ReadAllText(dotPath));
        Assert.False(File.Exists(Path.Combine(_directory, "MinionRelations.png")));
        Assert.Contains(logger.Infos, message => message.Contains("Skipping png render"));
    }

    [Fact]
    public async Task Execute_RendersImageWhenARendererIsAvailable()
    {
        var renderer = new FakeRenderer("fake", available: true);
        var step = CreateStep(new GraphvizRendererResolver([renderer]), new CapturingLogger(), "ItemsToMinions.png");

        await step.Execute(CreateContext(), Array.Empty<Entity<string>>());

        Assert.True(File.Exists(Path.Combine(_directory, "ItemsToMinions.dot")));
        Assert.True(renderer.Rendered);
        Assert.Equal(Path.Combine(_directory, "ItemsToMinions.png"), renderer.OutputPath);
        Assert.True(File.Exists(Path.Combine(_directory, "ItemsToMinions.png")));
    }

    [Fact]
    public async Task Execute_DoesNotFailThePipelineWhenRenderingThrows()
    {
        var renderer = new FakeRenderer("broken", available: true, throwOnRender: true);
        var logger = new CapturingLogger();
        var step = CreateStep(new GraphvizRendererResolver([renderer]), logger, "ApparatusRelations.dot");

        await step.Execute(CreateContext(), Array.Empty<Entity<string>>());

        Assert.True(File.Exists(Path.Combine(_directory, "ApparatusRelations.dot")));
        Assert.Contains(logger.Warnings, message => message.Contains("failed to render"));
    }

    private TestRenderGraphStep CreateStep(IGraphvizRendererResolver resolver, ILogger logger, string filename) =>
        new(
            new GraphSettings { OutputDirectory = _directory },
            resolver,
            logger,
            filename);

    private static PipelineContext<string> CreateContext() =>
        new(
            new ExecutionConfig(new FilesConfig()),
            new PipelineConfig<string>("String_Pipeline"),
            new CapturingLogger(),
            new NullDiagnostics());

    private sealed class TestRenderGraphStep : RenderGraphStep<string>
    {
        public TestRenderGraphStep(
            GraphSettings graphSettings,
            IGraphvizRendererResolver rendererResolver,
            ILogger logger,
            string filename)
            : base(graphSettings, rendererResolver, logger, filename)
        {
        }

        protected override Task Render(
            IPipelineContext<string> context,
            IReadOnlyCollection<Entity<string>> entities,
            RootGraph graph)
        {
            graph.GetOrAddNode("n");
            return Task.CompletedTask;
        }
    }

    private sealed class FakeRenderer : IGraphvizRenderer
    {
        private readonly bool _throwOnRender;

        public FakeRenderer(string name, bool available, bool throwOnRender = false)
        {
            Name = name;
            Available = available;
            _throwOnRender = throwOnRender;
        }

        public string Name { get; }

        public bool Available { get; }

        public bool Rendered { get; private set; }

        public string? OutputPath { get; private set; }

        public bool IsAvailable() => Available;

        public void RenderDot(string dotPath, string outputPath, string layoutEngine)
        {
            if (_throwOnRender)
            {
                throw new InvalidOperationException("render failed");
            }

            Rendered = true;
            OutputPath = outputPath;
            File.WriteAllText(outputPath, "png");
        }
    }

    private sealed class CapturingLogger : ILogger
    {
        public List<string> Infos { get; } = [];
        public List<string> Warnings { get; } = [];

        public void Info(string message) => Infos.Add(message);
        public void Warning(string message) => Warnings.Add(message);
        public void Error(string message) { }
        public void Success(string message) { }
        public void Verbose(string message) { }
    }

    private sealed class NullDiagnostics : IDiagnostics
    {
        public void Time(string operationDescription, Action operation) => operation();
        public T Time<T>(string operationDescription, Func<T> operation) => operation();
        public Task Time(string operationDescription, Func<Task> operation) => operation();
    }
}
