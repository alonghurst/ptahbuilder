using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Output;

public class SplitTsvIntoManyFilesStep : IStep<string>
{
    private readonly IFilesConfig _filesConfig;

    private const string SheetPrefix = "###Sheet: ";

    public SplitTsvIntoManyFilesStep(IFilesConfig filesConfig)
    {
        _filesConfig = filesConfig;
    }

    public Task Execute(IPipelineContext<string> context, IReadOnlyCollection<Entity<string>> entities)
    {
        foreach (var entity in entities) { 
            var content = entity.Value;

            var lines = new List<string>();
            var filename = "";

            void WriteFile()
            {
                if (!string.IsNullOrWhiteSpace(filename) && lines.Any())
                {
                    var file = Path.Combine(_filesConfig.DataDirectory, $"{filename}.tsv");

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