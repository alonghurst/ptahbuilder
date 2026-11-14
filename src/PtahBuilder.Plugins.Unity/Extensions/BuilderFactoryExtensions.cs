using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.Plugins.Unity;
using PtahBuilder.Plugins.Unity.Config;

namespace PtahBuilder.Plugins.Unity.Extensions;

/// <summary>
/// Extension methods for configuring Unity in a PtahBuilder pipeline.
/// </summary>
public static class BuilderFactoryExtensions
{
    /// <summary>
    /// Configures Unity project paths and registers <see cref="UnityConfig"/> in the builder's dependency injection pipeline.
    /// Generated JSON is replaced in place so Unity .meta files keep their guids.
    /// </summary>
    /// <param name="builderFactory">The builder factory.</param>
    /// <param name="rootPath">Root path (e.g. repository root). Used to derive default project directory when not overridden.</param>
    /// <param name="config">Optional config. Non-empty string properties override the defaults.</param>
    /// <returns>The same <see cref="BuilderFactory"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the resolved Unity project directory does not exist.</exception>
    public static BuilderFactory UseUnity(this BuilderFactory builderFactory, string rootPath, UnityConfig? config = null)
    {
        var projectDirectory = string.IsNullOrWhiteSpace(config?.ProjectDirectory)
            ? Path.Combine(rootPath, Path.GetFileName(rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) + ".Unity")
            : config.ProjectDirectory;

        var unityConfig = new UnityConfig
        {
            ProjectDirectory = Path.GetFullPath(projectDirectory)
        };

        if (!Directory.Exists(unityConfig.ProjectDirectory))
        {
            throw new InvalidOperationException(
                $"Unity project directory does not exist: {unityConfig.ProjectDirectory}. " +
                "Ensure the path is correct or create the Unity project before running the builder.");
        }

        var projectDirName = Path.GetFileName(unityConfig.ProjectDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var relativeOutputDirectory = Path.Combine(projectDirName, "Assets", "Resources", "Data");

        builderFactory.ConfigureFiles(x => x.Configure(rootPath, relativeOutputDirectory: relativeOutputDirectory));
        builderFactory.ConfigureServices(services => services.AddSingleton(unityConfig));
        KeepUnityOutputMetadata(builderFactory);

        return builderFactory;
    }

    /// <summary>
    /// Configures Unity project paths, registers <see cref="UnityConfig"/> in the builder's dependency injection pipeline,
    /// and runs the given <paramref name="configureFiles"/> action so file config can be set in the same call.
    /// Generated JSON is replaced in place so Unity .meta files keep their guids.
    /// </summary>
    /// <param name="builderFactory">The builder factory.</param>
    /// <param name="rootPath">Root path (e.g. repository root). Used to derive default project directory when not overridden.</param>
    /// <param name="configureFiles">Action to configure the builder's file config and Unity config (e.g. add additional directories).</param>
    /// <param name="config">Optional config. Non-empty string properties override the defaults.</param>
    /// <returns>The same <see cref="BuilderFactory"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the resolved Unity project directory does not exist.</exception>
    public static BuilderFactory UseUnity(this BuilderFactory builderFactory, string rootPath, Action<IFilesConfig, UnityConfig> configureFiles, UnityConfig? config = null)
    {
        var projectDirectory = string.IsNullOrWhiteSpace(config?.ProjectDirectory)
            ? Path.Combine(rootPath, Path.GetFileName(rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) + ".Unity")
            : config.ProjectDirectory;

        var unityConfig = new UnityConfig
        {
            ProjectDirectory = Path.GetFullPath(projectDirectory)
        };

        if (!Directory.Exists(unityConfig.ProjectDirectory))
        {
            throw new InvalidOperationException(
                $"Unity project directory does not exist: {unityConfig.ProjectDirectory}. " +
                "Ensure the path is correct or create the Unity project before running the builder.");
        }

        var projectDirName = Path.GetFileName(unityConfig.ProjectDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var relativeOutputDirectory = Path.Combine(projectDirName, "Assets", "Resources", "Data");

        builderFactory.ConfigureFiles(x =>
        {
            x.Configure(rootPath, relativeOutputDirectory: relativeOutputDirectory);
            configureFiles(x, unityConfig);
        });
        builderFactory.ConfigureServices(services => services.AddSingleton(unityConfig));
        KeepUnityOutputMetadata(builderFactory);

        return builderFactory;
    }

    /// <summary>
    /// JSON the run did not write is removed, along with <c>.meta</c> files whose assets are gone.
    /// Rewritten JSON keeps its <c>.meta</c> guid. The output directory itself is left in place.
    /// </summary>
    private static void KeepUnityOutputMetadata(BuilderFactory builderFactory)
    {
        builderFactory.AfterSuccessfulRun((config, writtenAfterUtc) =>
            UnityGeneratedOutput.RemoveStale(config.Files.OutputDirectory, writtenAfterUtc));
    }
}
