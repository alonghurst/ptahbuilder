using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Steps.Input;
using PtahBuilder.BuildSystem.Steps.Output.SimpleText;
using PtahBuilder.Generators.ComponentModelDocumentation.Config;
using PtahBuilder.Generators.ComponentModelDocumentation.Entities;
using PtahBuilder.Generators.ComponentModelDocumentation.Steps;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Extensions;

public static class ExecutionConfigExtensions
{
    public static ExecutionConfig AddComponentModelDocumentationPipeline(this ExecutionConfig config, Action<DocumentationConfig> configure)
    {
        var documentationConfig = new DocumentationConfig();

        configure.Invoke(documentationConfig);

        var types = documentationConfig
            .GetTypesToDocument()
            .Select(x => new TypeToDocument(x))
            .ToList();

        config.AddPipelinePhase(phase =>
        {
            phase.AddPipeline<TypeToDocument>(p =>
            {
                p.DuplicateIdBehaviour = DuplicateIdBehaviour.Skip;
                p.GetId = t => t.Type.GetTypeName();

                p.AddInputStep<InsertEntitiesStep<TypeToDocument>>(types);
                p.AddInputStep<FindAdditionalTypesToDocumentStep>();
            });
            
            phase.AddPipeline<SimpleTextOutput>(c =>
            {
                c.AddProcessStep<CreateDocumentationStep>();

                c.AddOutputStep<SimpleTextOutputStep>();
            });
        });
        
        return config;
    }
}