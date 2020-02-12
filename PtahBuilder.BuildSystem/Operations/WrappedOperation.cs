using System;
using System.Collections.Generic;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Operations
{
    public class WrappedOperation<T> : IOperation<T> where T : new()
    {
        public Action<Dictionary<T, MetadataCollection>> Operate { get; set; }

        Dictionary<T, MetadataCollection> IOperation<T>.Operate(Dictionary<T, MetadataCollection> entities)
        {
            Operate(entities);
            return entities;
        }
    }
}
