﻿using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Extensions;

public static class PipelineContextExtensions
{
    public static void AddEntityFromFile<T>(this IPipelineContext<T> context, T entity, string filename, Dictionary<string, object>? metadata = null)
    {
        metadata ??= new Dictionary<string, object>();

        metadata.Add(MetadataKeys.SourceFile, filename);

        context.AddEntity(entity, metadata);
    }
}