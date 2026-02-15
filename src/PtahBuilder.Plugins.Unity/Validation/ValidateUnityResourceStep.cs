using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Plugins.Unity.Config;

namespace PtahBuilder.Plugins.Unity.Validation;

/// <summary>
/// Validates that a Unity Resource file exists per entity, under a given subdirectory of Resources (by filename and extension).
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public class ValidateUnityResourceStep<T> : IStep<T>
{
    private readonly UnityConfig _unityConfig;
    private readonly string _subdirectory;
    private readonly string _extension;
    private readonly Func<Entity<T>, bool>? _shouldProcess;
    private readonly Func<Entity<T>, string?>? _fileNameAccessor;

    /// <summary>
    /// Constructor with optional shouldProcess and fileNameAccessor.
    /// </summary>
    public ValidateUnityResourceStep(
        UnityConfig unityConfig,
        string subdirectory,
        string extension = "prefab",
        Func<Entity<T>, bool>? shouldProcess = null,
        Func<Entity<T>, string?>? fileNameAccessor = null)
    {
        _unityConfig = unityConfig;
        _subdirectory = subdirectory;
        _extension = extension;
        _shouldProcess = shouldProcess;
        _fileNameAccessor = fileNameAccessor;
    }

    /// <summary>
    /// Constructor with only fileNameAccessor (no shouldProcess). Use when filename comes from entity and all entities are processed.
    /// </summary>
    public ValidateUnityResourceStep(
        UnityConfig unityConfig,
        string subdirectory,
        string extension,
        Func<Entity<T>, string?> fileNameAccessor)
    {
        _unityConfig = unityConfig;
        _subdirectory = subdirectory;
        _extension = extension;
        _shouldProcess = null;
        _fileNameAccessor = fileNameAccessor;
    }

    /// <inheritdoc />
    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var basePath = Path.Combine(_unityConfig.Resources, _subdirectory);

        foreach (var entity in entities)
        {
            if (_shouldProcess != null && !_shouldProcess(entity))
                continue;

            string? fileName = _fileNameAccessor != null
                ? _fileNameAccessor(entity)
                : entity.Id;

            if (string.IsNullOrWhiteSpace(fileName))
                continue;

            var desiredPath = Path.Combine(basePath, $"{fileName}.{_extension}");
            var alternatePath = Path.Combine(basePath, fileName, $"{fileName}.{_extension}");

            if (!File.Exists(desiredPath) && !File.Exists(alternatePath))
                context.AddValidationError(entity, this, $"Resource does not exist at {desiredPath}");
        }

        return Task.CompletedTask;
    }
}
