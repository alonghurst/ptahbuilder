using System;
using PtahBuilder.BuildSystem.Generators;
using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Generators.Operations;
using PtahBuilder.TestBuilder.Types;

namespace PtahBuilder.TestBuilder.AdditionalOperations;

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