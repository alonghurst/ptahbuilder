using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.Plugins.GraphViz.Config;

namespace PtahBuilder.Plugins.GraphViz.Extensions;

public static class BuilderFactoryExtensions
{
    public static BuilderFactory UseGraphViz(this BuilderFactory builderFactory, string rootPath, GraphSettings? settings = null)
    {
        return builderFactory.UseGraphViz(rootPath, (_, _) => { }, settings);
    }

    public static BuilderFactory UseGraphViz(this BuilderFactory builderFactory, string rootPath, Action<IFilesConfig, GraphSettings> configureFiles, GraphSettings? settings = null)
    {
        var graphSettings = settings ?? new GraphSettings();

        if (string.IsNullOrWhiteSpace(graphSettings.OutputDirectory))
        {
            graphSettings.OutputDirectory = Path.GetFullPath(Path.Combine(rootPath, "Output", "Graphs"));
        }

        builderFactory.ConfigureFiles(x =>
        {
            configureFiles(x, graphSettings);
        });
        builderFactory.ConfigureServices(services => services.AddSingleton(graphSettings));

        return builderFactory;
    }
}
