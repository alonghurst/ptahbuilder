namespace PtahBuilder.Plugins.Unity.Validation;

internal static class UnityResourceFiles
{
    public static bool Exists(string basePath, string fileName, string extension, bool considerSubfolders)
    {
        var file = $"{fileName}.{extension}";

        if (File.Exists(Path.Combine(basePath, file)))
            return true;

        if (File.Exists(Path.Combine(basePath, fileName, file)))
            return true;

        if (!considerSubfolders || !Directory.Exists(basePath))
            return false;

        return Directory.EnumerateFiles(basePath, file, SearchOption.AllDirectories).Any();
    }
}
