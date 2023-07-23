using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Generators.ComponentModelDocumentation.Entities;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Steps
{
    public class TypeToDocumentationStep : IStep<TypeDocumentation>
    {
        private readonly IEntityProvider<Type> _entityProvider;

        public TypeToDocumentationStep(IEntityProvider<Type> entityProvider)
        {
            _entityProvider = entityProvider;
        }

        public Task Execute(IPipelineContext<TypeDocumentation> context, IReadOnlyCollection<Entity<TypeDocumentation>> entities)
        {
            throw new NotImplementedException();
        }
    }
}
