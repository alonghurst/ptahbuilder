using System;
using System.Collections.Generic;
using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Generators;
using PtahBuilder.BuildSystem.Metadata;
using PtahBuilder.BuildSystem.Operations;
using PtahBuilder.TestBuilder.Types;

namespace PtahBuilder.TestBuilder
{
    public static class ExtendedConfig
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
}
