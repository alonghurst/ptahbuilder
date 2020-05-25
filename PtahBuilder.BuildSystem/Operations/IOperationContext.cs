using System;
using System.Collections.Generic;
using System.Text;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Operations
{
    public interface IOperationContext<T>
    {
        public Logger Logger { get; }
        public PathResolver PathResolver { get; }
        public Dictionary<T, MetadataCollection> Entities { get; }
        public BaseDataMetadataResolver<T> MetadataResolver { get; }
    }
}
