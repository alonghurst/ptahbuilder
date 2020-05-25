using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Operations;
using PtahBuilder.BuildSystem.Syntax;

namespace PtahBuilder.BuildSystem.Generators
{
    public class OperationProvider<T> : WithOperationContext<T>
    {
        public OperationProvider(IOperationContext<T> context) : base(context)
        {
        }

        public IEnumerable<Operation<T>> BuildOperations()
        {
            yield return new FileMover<T>(Logger, PathResolver, MetadataResolver);

            var operations = GetOperations().ToArray();

            foreach (var operation in operations)
            {
                yield return operation;
            }
        }

        protected virtual IEnumerable<Operation<T>> GetOperations()
        {
            yield break;
        }
    }
}
