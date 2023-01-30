using PtahBuilder.LegacyBuildSystem.Generators;
using PtahBuilder.LegacyBuildSystem.Generators.Context;
using PtahBuilder.LegacyBuildSystem.Generators.Operations;
using PtahBuilder.Tests.LegacyTestBuilder.Types;

namespace PtahBuilder.Tests.LegacyTestBuilder.AdditionalOperations;

public static class ReverseNameOperation
{
    public class SimpleTypeOperationProvider : Operation<SimpleType>
    {
        public SimpleTypeOperationProvider(IOperationContext<SimpleType> context) : base(context)
        {
        }

        [Operate]
        public void ReverseName()
        {
            foreach (var entity in Entities)
            {
                if (entity.Value.ContainsKey("ReverseName"))
                {
                    char[] arr = entity.Key.Name.ToCharArray();
                    Array.Reverse(arr);
                    entity.Key.Name = new string(arr);
                }
            }
        }


    }
}