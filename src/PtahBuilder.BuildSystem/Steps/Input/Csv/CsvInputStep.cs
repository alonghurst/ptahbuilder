using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Input.Csv;

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


public class CsvInputStep<T> : CsvReadStep<T> where T : class
{
    private readonly CsvMapping<T> _mapping;
    private readonly IDynamicMappingService _dynamicMappingService;
    private readonly Action<T>? _postEntityRead;

    public CsvInputStep(IFilesConfig filesConfig, ILogger logger, string fileName, CsvMapping<T> mapping, IDynamicMappingService dynamicMappingService,  CsvReadOptions? options = null, Action<T>? postEntityRead = null)
    : base(filesConfig, logger, fileName, options)
    {
        _mapping = mapping;
        _dynamicMappingService = dynamicMappingService;
        _postEntityRead = postEntityRead;
    }

    protected override void RowReadFromFile(IPipelineContext<T> context, ReadRow readRow)
    {
        var columns = readRow.Columns;
        var entity = _mapping.Instantiate();

        for (int i = 0; i < columns.Length && i < _mapping.ColumnPropertyNames.Length; i++)
        {
            var propertyName = _mapping.ColumnPropertyNames[i];

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                continue;
            }

            var value = columns[i];

            _dynamicMappingService.Map(entity, propertyName, value);
        }
        
        context.AddEntityFromFile(entity, readRow.Filename);

        _postEntityRead?.Invoke(entity);
    }
}