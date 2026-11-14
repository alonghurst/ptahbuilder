namespace PtahBuilder.Plugins.Unity;

/// <summary>
/// Removes generated JSON the latest run did not write, then drops <c>.meta</c> files whose asset is gone.
/// Rewritten JSON keeps the <c>.meta</c> guid beside it.
/// </summary>
public static class UnityGeneratedOutput
{
    public static void RemoveStale(string outputDirectory, DateTime writtenAfterUtc)
    {
        if (string.IsNullOrWhiteSpace(outputDirectory) || !Directory.Exists(outputDirectory))
            return;

        foreach (var file in Directory.EnumerateFiles(outputDirectory, "*.json", SearchOption.AllDirectories).ToArray())
        {
            // Windows wildcard matching can also return *.json.meta.
            if (!file.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                continue;

            if (File.GetLastWriteTimeUtc(file) < writtenAfterUtc)
                File.Delete(file);
        }

        DeleteOrphanMetas(outputDirectory);
        DeleteEmptyDirectories(outputDirectory);
        DeleteOrphanMetas(outputDirectory);
    }

    private static void DeleteOrphanMetas(string outputDirectory)
    {
        foreach (var meta in Directory.EnumerateFiles(outputDirectory, "*.meta", SearchOption.AllDirectories).ToArray())
        {
            if (!meta.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                continue;

            var assetPath = meta[..^".meta".Length];
            if (!File.Exists(assetPath) && !Directory.Exists(assetPath))
                File.Delete(meta);
        }
    }

    private static void DeleteEmptyDirectories(string outputDirectory)
    {
        var directories = Directory.EnumerateDirectories(outputDirectory, "*", SearchOption.AllDirectories)
            .OrderByDescending(path => path.Length)
            .ToArray();

        foreach (var directory in directories)
        {
            if (!Directory.EnumerateFileSystemEntries(directory).Any())
                Directory.Delete(directory);
        }
    }
}
