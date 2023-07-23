using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Steps.Input;
using PtahBuilder.Generators.ComponentModelDocumentation.Config;
using PtahBuilder.Generators.ComponentModelDocumentation.Steps;
using PtahBuilder.Util.Extensions;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Extensions
{
    public static class ExecutionConfigExtensions
    {
        public static ExecutionConfig AddComponentModelDocumentationPipeline(this ExecutionConfig config, Action<DocumentationConfig> configure)
        {
            var documentationConfig = new DocumentationConfig();

            configure.Invoke(documentationConfig);

            var types = documentationConfig.GetTypesToDocument();

            config.AddPipelinePhase(phase =>
            {
                phase.AddPipeline<Type>(p =>
                {
                    p.DuplicateIdBehaviour = DuplicateIdBehaviour.Skip;
                    p.GetId = t => t.GetTypeName();

                    p.AddInputStep<InsertEntitiesStep<Type>>(types);

                    p.AddInputStep<FindAdditionalTypesToDocumentStep>();
                });


            });

            return config;
        }
    }
}
