using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Output.SimpleText;

public class SimpleTextOutputStep:IStep<SimpleTextOutput>
{
    public Task Execute(IPipelineContext<SimpleTextOutput> context, IReadOnlyCollection<Entity<SimpleTextOutput>> entities)
    {
        foreach (var entity in entities)
        {
            var filename = Path.Combine(entity.Value.Path, $"{entity.Value.Name}.{entity.Value.Extension}");

            if (!Directory.Exists(Path.GetDirectoryName(filename)))
            {
                Directory.CreateDirectory(filename);
            }

            File.WriteAllText(filename, entity.Value.Contents);
        }

        return Task.CompletedTask;
    }
}