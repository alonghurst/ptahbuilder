using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.Generators.ComponentModelDocumentation.Config;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Extensions
{
    public static class ExecutionConfigExtensions
    {
        public static ExecutionConfig AddComponentModelDocumentationPipeline(this ExecutionConfig config, Action<DocumentationConfig> configure)
        {
            var documentationConfig = new DocumentationConfig();

            configure.Invoke(documentationConfig);

            return config;
        }
    }
}
