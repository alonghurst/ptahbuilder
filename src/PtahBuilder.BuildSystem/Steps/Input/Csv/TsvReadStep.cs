using PtahBuilder.BuildSystem.Config;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Input.Csv;

public abstract class TsvReadStep<T> : CsvReadStep<T> where T : class
{
    public TsvReadStep(IFilesConfig filesConfig, ILogger logger, string fileName, CsvReadOptions? options = null) 
        : base(filesConfig, logger, fileName, FixOptions(options))
    {
    }

    private static CsvReadOptions FixOptions(CsvReadOptions? options)
    {
        options ??= new();

        options.ColumnSeparator = "\t";

        return options;
    }
}