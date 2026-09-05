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
    private readonly bool _considerSubfolders;

    /// <summary>
    /// Constructor with optional shouldProcess and fileNameAccessor.
    /// </summary>
    /// <param name="considerSubfolders">
    /// When true, a matching file in any nested folder under <paramref name="subdirectory"/> is accepted.
    /// When false, only <c>{subdirectory}/{fileName}.{extension}</c> and
    /// <c>{subdirectory}/{fileName}/{fileName}.{extension}</c> are checked.
    /// </param>
    public ValidateUnityResourceStep(
        UnityConfig unityConfig,
        string subdirectory,
        string extension = "prefab",
        Func<Entity<T>, bool>? shouldProcess = null,
        Func<Entity<T>, string?>? fileNameAccessor = null,
        bool considerSubfolders = false)
    {
        _unityConfig = unityConfig;
        _subdirectory = subdirectory;
        _extension = extension;
        _shouldProcess = shouldProcess;
        _fileNameAccessor = fileNameAccessor;
        _considerSubfolders = considerSubfolders;
    }

    /// <summary>
    /// Constructor with only fileNameAccessor (no shouldProcess). Use when filename comes from entity and all entities are processed.
    /// </summary>
    public ValidateUnityResourceStep(
        UnityConfig unityConfig,
        string subdirectory,
        string extension,
        Func<Entity<T>, string?> fileNameAccessor)
        : this(unityConfig, subdirectory, extension, shouldProcess: null, fileNameAccessor)
    {
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

            if (UnityResourceFiles.Exists(basePath, fileName, _extension, _considerSubfolders))
                continue;

            var desiredPath = Path.Combine(basePath, $"{fileName}.{_extension}");
            var location = _considerSubfolders
                ? $"{desiredPath} or in subfolders of {basePath}"
                : desiredPath;

            context.AddValidationError(entity, this, $"Resource does not exist at {location}");
        }

        return Task.CompletedTask;
    }
}
