using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Input;

public class CsvReadOptions
{
    public bool SkipFirstLine { get; set; } = true;
    public string ColumnSeparator { get; set; } = ",";
}

public class CsvMapping<T>
{
    public CsvMapping(Func<T> instantiate, params string[] columnPropertyNames)
    {
        Instantiate = instantiate;
        ColumnPropertyNames = columnPropertyNames;
    }

    public Func<T> Instantiate { get; }
    public string[] ColumnPropertyNames { get; }
}


public class CsvInputStep<T> : IStep<T> where T : class
{
    private readonly ILogger _logger;
    private readonly IFilesConfig _filesConfig;
    private readonly string _fileName;
    private readonly CsvReadOptions _options;
    private readonly CsvMapping<T> _mapping;
    private readonly IDynamicMappingService _dynamicMappingService;

    public CsvInputStep(IFilesConfig filesConfig, ILogger logger, string fileName, CsvMapping<T> mapping, IDynamicMappingService dynamicMappingService, CsvReadOptions? options = null)
    {
        _filesConfig = filesConfig;
        _logger = logger;
        _fileName = fileName;
        _options = options ?? new CsvReadOptions();
        _mapping = mapping;
        _dynamicMappingService = dynamicMappingService;
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

            var entity = _mapping.Instantiate();
            
            for (int i = 0; i < columns.Length && i < _mapping.ColumnPropertyNames.Length; i++)
            {
                var propertyName = _mapping.ColumnPropertyNames[i];

                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    continue;
                }

                var value = columns[i];

                _logger.Info(value);

                _dynamicMappingService.Map(entity, propertyName, value);
            }

            context.AddEntityFromFile(entity, file);
        }
    }
}