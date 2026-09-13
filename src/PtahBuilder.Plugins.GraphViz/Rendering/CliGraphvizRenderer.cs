using System.Diagnostics;

namespace PtahBuilder.Plugins.GraphViz.Rendering;

public class CliGraphvizRenderer : IGraphvizRenderer
{
    private readonly Func<string?> _findDot;

    public CliGraphvizRenderer()
        : this(FindDotOnPath)
    {
    }

    internal CliGraphvizRenderer(Func<string?> findDot)
    {
        ArgumentNullException.ThrowIfNull(findDot);
        _findDot = findDot;
    }

    public string Name => "Graphviz CLI";

    public bool IsAvailable() => !string.IsNullOrWhiteSpace(_findDot());

    public void RenderDot(string dotPath, string outputPath, string layoutEngine)
    {
        Require.NotNullOrWhiteSpace(dotPath, nameof(dotPath));
        Require.NotNullOrWhiteSpace(outputPath, nameof(outputPath));

        var executable = _findDot();
        if (string.IsNullOrWhiteSpace(executable))
        {
            throw new InvalidOperationException($"{Name} is not available because 'dot' was not found on PATH.");
        }

        var engine = string.IsNullOrWhiteSpace(layoutEngine) ? "dot" : layoutEngine;
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            ArgumentList = { "-Tpng", $"-K{engine}", dotPath, "-o", outputPath },
            RedirectStandardError = true,
            RedirectStandardOutput = false,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        var standardError = process.StandardError.ReadToEnd();
        if (!process.WaitForExit(60_000))
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch
            {
                // ignored
            }

            throw new TimeoutException($"{Name} timed out while rendering '{dotPath}'.");
        }

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"{Name} failed with exit code {process.ExitCode} while rendering '{dotPath}'. {standardError}".Trim());
        }
    }

    internal static string? FindDotOnPath()
    {
        var fileName = OperatingSystem.IsWindows() ? "dot.exe" : "dot";
        var path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        foreach (var directory in path.Split(Path.PathSeparator))
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                continue;
            }

            var candidate = Path.Combine(directory, fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }
}
