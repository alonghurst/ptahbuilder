using System;
using System.Collections.Generic;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Operations
{
    public class WrappedOperation<T> : IOperation<T> where T : new()
    {
        public WrappedOperation(Action<Dictionary<T, MetadataCollection>> operate)
        {
            Operate = operate;
        }

        public Action<Dictionary<T, MetadataCollection>> Operate { get;}

        Dictionary<T, MetadataCollection> IOperation<T>.Operate(Dictionary<T, MetadataCollection> entities)
        {
            Operate(entities);
            return entities;
        }
    }
}
