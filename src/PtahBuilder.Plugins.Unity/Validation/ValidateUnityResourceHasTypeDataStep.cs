using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Plugins.Unity.Config;

namespace PtahBuilder.Plugins.Unity.Validation;

/// <summary>
/// Checks that every Unity content resource has a corresponding TypeData entity.
///
/// This is the reverse of <c>ValidateUnityResourceStep</c>:
/// that step validates that each TypeData entity has a Unity resource,
/// while this step validates that each Unity resource has a TypeData entity.
/// </summary>
public sealed class ValidateUnityResourceHasTypeDataStep<TPrimary> : IStep<TPrimary>
{
    private readonly UnityConfig _unity;
    private readonly string _contentSubdirectory;
    private readonly string _extension;
    private readonly string[] _ignoreIds;
    private readonly string[] _ignorePrefixes;

    public ValidateUnityResourceHasTypeDataStep(
        UnityConfig unity,
        string contentSubdirectory,
        string extension,
        string[]? ignoreIds = null,
        string[]? ignorePrefixes = null)
    {
        _unity = unity;
        _contentSubdirectory = contentSubdirectory;
        _extension = extension;
        _ignoreIds = ignoreIds ?? [];
        _ignorePrefixes = ignorePrefixes ?? [];
    }

    public Task Execute(
        IPipelineContext<TPrimary> context,
        IReadOnlyCollection<Entity<TPrimary>> entities)
    {
        var typeDataIds = entities
            .Select(e => e.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        UnityResourceHasTypeDataValidation.ReportMissing(
            context,
            this,
            _unity,
            _contentSubdirectory,
            _extension,
            typeDataIds,
            _ignoreIds,
            _ignorePrefixes);

        return Task.CompletedTask;
    }
}

/// <summary>
/// Variant that validates a Unity content folder against a union of two TypeData entity sets.
/// Useful when multiple TypeData types share the same Unity content folder (e.g. Enemies + Bosses).
/// </summary>
public sealed class ValidateUnityResourceHasTypeDataStep<TPrimary, TSecondary> : IStep<TPrimary>
{
    private readonly UnityConfig _unity;
    private readonly IEntityProvider<TSecondary> _secondary;
    private readonly string _contentSubdirectory;
    private readonly string _extension;
    private readonly string[] _ignoreIds;
    private readonly string[] _ignorePrefixes;

    public ValidateUnityResourceHasTypeDataStep(
        UnityConfig unity,
        IEntityProvider<TSecondary> secondary,
        string contentSubdirectory,
        string extension,
        string[]? ignoreIds = null,
        string[]? ignorePrefixes = null)
    {
        _unity = unity;
        _secondary = secondary;
        _contentSubdirectory = contentSubdirectory;
        _extension = extension;
        _ignoreIds = ignoreIds ?? [];
        _ignorePrefixes = ignorePrefixes ?? [];
    }

    public Task Execute(
        IPipelineContext<TPrimary> context,
        IReadOnlyCollection<Entity<TPrimary>> entities)
    {
        var typeDataIds = entities
            .Select(e => e.Id)
            .Concat(_secondary.Entities.Values.Select(x => x.Id))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        UnityResourceHasTypeDataValidation.ReportMissing(
            context,
            this,
            _unity,
            _contentSubdirectory,
            _extension,
            typeDataIds,
            _ignoreIds,
            _ignorePrefixes);

        return Task.CompletedTask;
    }
}

internal static class UnityResourceHasTypeDataValidation
{
    public static void ReportMissing<T>(
        IPipelineContext<T> context,
        IStep<T> step,
        UnityConfig unity,
        string contentSubdirectory,
        string extension,
        ISet<string> typeDataIds,
        IReadOnlyCollection<string> ignoreIds,
        IReadOnlyCollection<string> ignorePrefixes)
    {
        var basePath = Path.Combine(unity.Resources, contentSubdirectory);
        if (!Directory.Exists(basePath))
        {
            context.AddPipelineValidationError(
                step,
                $"Unity content validation: directory '{basePath}' does not exist (subdirectory '{contentSubdirectory}').");
            return;
        }

        // `ValidateUnityResourceStep` supports two file layouts:
        // - {Resources}/{subdirectory}/{id}.{extension}
        // - {Resources}/{subdirectory}/{id}/{id}.{extension}
        // Enumerating all files under {subdirectory} lets us validate both without duplicating logic.
        var pattern = $"*.{extension}";
        var unityIds = Directory.EnumerateFiles(basePath, pattern, SearchOption.AllDirectories)
            .Select(file => Path.GetFileNameWithoutExtension(file))
            .Where(id => id is { Length: > 0 })
            .Select(id => id!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var ignoreIdSet = new HashSet<string>(ignoreIds, StringComparer.OrdinalIgnoreCase);

        var missing = unityIds
            .Where(id =>
                !ignoreIdSet.Contains(id) &&
                !ignorePrefixes.Any(prefix => id.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) &&
                !typeDataIds.Contains(id));

        foreach (var id in missing)
        {
            context.AddPipelineValidationError(
                step,
                $"Unity resource '{id}' in '{contentSubdirectory}' has no corresponding TypeData entity.");
        }
    }
}
