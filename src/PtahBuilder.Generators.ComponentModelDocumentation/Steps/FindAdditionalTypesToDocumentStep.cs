using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Steps
{
    public class FindAdditionalTypesToDocumentStep : IStep<Type>
    {
        public Task Execute(IPipelineContext<Type> context, IReadOnlyCollection<Entity<Type>> entities)
        {
            foreach (var entity in entities)
            {
                var type = entity.Value;

                var relevantProperties = type.get
            }
        }
    }
}
