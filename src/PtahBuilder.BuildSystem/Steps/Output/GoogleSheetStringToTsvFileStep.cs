using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace MarcoYolo.DataGenerator.Steps.DownloadDataSheets;

public class GoogleSheetStringToTsvFileStep : IStep<string>
{
    private readonly IFilesConfig _filesConfig;

    public GoogleSheetStringToTsvFileStep(IFilesConfig filesConfig)
    {
        _filesConfig = filesConfig;
    }

    public Task Execute(IPipelineContext<string> context, IReadOnlyCollection<Entity<string>> entities)
    {
        var tasks = entities.Select(x =>
        {
            var content = x.Value;
            var url = x.Id;

            var filename = url.Split("sheet=", StringSplitOptions.RemoveEmptyEntries)[1];

            var file = Path.Combine(_filesConfig.DataDirectory, $"{filename}.tsv");

            return File.WriteAllTextAsync(file, content);
        });

        return Task.WhenAll(tasks);
    }
}