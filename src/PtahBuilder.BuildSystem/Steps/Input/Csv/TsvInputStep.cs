using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Input.Csv;

public class TsvInputStep<T> : CsvInputStep<T> where T : class
{
    public TsvInputStep(IFilesConfig filesConfig, ILogger logger, string fileName, CsvMapping<T> mapping, IDynamicMappingService dynamicMappingService, CsvReadOptions? options = null) : base(filesConfig, logger, fileName, mapping, dynamicMappingService, FixOptions(options))
    {
    }

    private static CsvReadOptions FixOptions(CsvReadOptions? options)
    {
        options = options ?? new CsvReadOptions();

        options.ColumnSeparator = "\t";

        return options;
    }
}