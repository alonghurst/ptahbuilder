using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Output;

public class SplitTsvIntoManyFilesStep : IStep<string>
{
    private readonly IFilesConfig _filesConfig;
    private readonly string? _outputDirectory;

    private const string SheetPrefix = "###Sheet: ";

    public SplitTsvIntoManyFilesStep(IFilesConfig filesConfig, string? outputDirectory = null)
    {
        _filesConfig = filesConfig;
        _outputDirectory = outputDirectory;
    }

    public Task Execute(IPipelineContext<string> context, IReadOnlyCollection<Entity<string>> entities)
    {
        var output = string.IsNullOrWhiteSpace(_outputDirectory) ? _filesConfig.DataDirectory : _outputDirectory;

        foreach (var entity in entities)
        {
            var content = entity.Value;

            var lines = new List<string>();
            var filename = "";

            void WriteFile()
            {
                if (!string.IsNullOrWhiteSpace(filename) && lines.Any())
                {
                    var file = Path.Combine(output, $"{filename}.tsv");

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