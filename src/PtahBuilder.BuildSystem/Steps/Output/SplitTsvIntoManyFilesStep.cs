using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Output;

public class SplitTsvIntoManyFilesStep : IStep<string>
{
    private readonly IFilesConfig _filesConfig;
    private readonly string? _outputDirectory;
    private readonly Dictionary<string, string> _prefixLookup;

    private const string SheetPrefix = "###Sheet: ";

    public SplitTsvIntoManyFilesStep(IFilesConfig filesConfig, string? outputDirectory = null, Dictionary<string, string>? prefixLookup = null)
    {
        _filesConfig = filesConfig;
        _outputDirectory = outputDirectory;
        _prefixLookup= prefixLookup ?? new Dictionary<string, string>();
    }

    public Task Execute(IPipelineContext<string> context, IReadOnlyCollection<Entity<string>> entities)
    {
        var output = string.IsNullOrWhiteSpace(_outputDirectory) ? _filesConfig.DataDirectory : _outputDirectory;

        foreach (var entity in entities)
        {
            _prefixLookup.TryGetValue(entity.Id, out var prefix);

            if (!string.IsNullOrWhiteSpace(prefix))
            {
                prefix = $"{prefix} - ";
            }

            var content = entity.Value;

            var lines = new List<string>();
            var filename = "";

            void WriteFile()
            {
                if (!string.IsNullOrWhiteSpace(filename) && lines.Any())
                {
                    var file = Path.Combine(output, $"{prefix}{filename}.tsv");

                    File.WriteAllLines(file, lines);

                    lines.Clear();
                }
            }

            foreach (var line in content.Split('\n'))
            {
                if (line.StartsWith("###Sheet: "))
                {
                    WriteFile();
                    filename = line.Replace(SheetPrefix, string.Empty).Trim();
                }
                else
                {
                    lines.Add(line);
                }
            }

            WriteFile();
        }

        return Task.CompletedTask;
    }
}