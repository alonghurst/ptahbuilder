using System.Collections.Generic;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Operations
{
    public interface IOperation<T> where T : TypeData
    {
        Dictionary<T, MetadataCollection> Operate(Dictionary<T, MetadataCollection> entities);
    }
}
