using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.TestBuilder
{
   public static class MinimalConfig
    {
        public class Files : IFiles
        {
            public string Root => ".";
            public string OutputForCode => "./Output";
        }

        public class MetadataResolver<T> : BaseDataMetadataResolver<T> where T : BaseTypeData
        {
            public override string AbsoluteNamespaceForOutput => "PtahBuilder.TestBuilder.Output";

            public override string GetEntityTypeName(T entity) => entity.TypeName;
        }
    }
}
