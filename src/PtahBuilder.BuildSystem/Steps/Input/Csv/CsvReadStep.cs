using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Input.Csv;

public class CsvReadOptions
{
    public bool SkipFirstLine { get; set; } = true;
    public string ColumnSeparator { get; set; } = ",";
}

public abstract class CsvReadStep<T> : IStep<T> where T : class
{
    private readonly ILogger _logger;
    private readonly IFilesConfig _filesConfig;
    private readonly string _fileName;
    private readonly CsvReadOptions _options;

    public CsvReadStep(IFilesConfig filesConfig, ILogger logger, string fileName, CsvReadOptions? options = null)
    {
        _filesConfig = filesConfig;
        _logger = logger;
        _fileName = fileName;
        _options = options ?? new CsvReadOptions();
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var file = Path.Combine(_filesConfig.DataDirectory, _fileName);

        _logger.Verbose($"Reading {file}");

        var lines = await File.ReadAllLinesAsync(file);
        var hasSkipped = false;

        foreach (var line in lines)
        {
            if (_options.SkipFirstLine && !hasSkipped)
            {
                hasSkipped = true;
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var columns = line.Split(_options.ColumnSeparator, StringSplitOptions.TrimEntries);

            if (columns.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            RowReadFromFile(context, new ReadRow(file, line, columns));
        }
    }

    protected abstract void RowReadFromFile(IPipelineContext<T> context, ReadRow readRow);

    public record ReadRow(string Filename, string Row, string[] Columns);
}