namespace PtahBuilder.Util.Helpers;

public static class PathHelper
{
    public static string GetRootPath(string lookingForDirectory, params string[] args)
    {
        string root = string.Empty;

        if (args.Length > 0)
        {
            root = args[0];
        }

        var path = string.IsNullOrWhiteSpace(root) ? ".." : root;
        path = Path.GetFullPath(path);

        while (Directory.GetDirectories(path).All(x => !x.EndsWith(lookingForDirectory)))
        {
            var parent = Path.GetFullPath(Path.Combine(path, ".."));

            if (string.Equals(path, parent, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Reached filesystem root without finding a directory ending with '{lookingForDirectory}'.");
            }

            path = parent;
            Console.WriteLine($"Trying {path}");
        }

        root = Path.GetFullPath(path);

        return root;
    }
}