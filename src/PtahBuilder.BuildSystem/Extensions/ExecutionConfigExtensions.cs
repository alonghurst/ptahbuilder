using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Steps.Output.SimpleText;

namespace PtahBuilder.BuildSystem.Extensions;

public static class ExecutionConfigExtensions
{
    public static ExecutionConfig WriteSimpleTextFiles(this ExecutionConfig config)
    {
        config.AddPipelinePhase(phase =>
        {
            phase.AddPipeline<SimpleTextOutput>(p =>
            {
                p.AddOutputStep<SimpleTextOutputStep>();
            });
        });

        return config;
    }
}