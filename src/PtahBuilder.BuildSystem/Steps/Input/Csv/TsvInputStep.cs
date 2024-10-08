﻿using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Input.Csv;

public class TsvInputStep<T> : CsvInputStep<T> where T : class
{
    public TsvInputStep(IFilesConfig filesConfig, ILogger logger, string fileName, CsvMapping<T> mapping, IDynamicMappingService dynamicMappingService, CsvReadOptions? options = null, Action<T>? postEntityRead = null)
        : base(filesConfig, logger, fileName, mapping, dynamicMappingService, FixOptions(options), postEntityRead)
    {
    }

    private static CsvReadOptions FixOptions(CsvReadOptions? options)
    {
        options ??= new();

        options.ColumnSeparator = "\t";

        return options;
    }
}