using System.Collections.Generic;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Operations
{
    public interface IOperation<T> 
    {
        Dictionary<T, MetadataCollection> Operate(Dictionary<T, MetadataCollection> entities);
    }
}
