using System.Collections.Generic;
using System.Linq;
using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Generators.Operations;

namespace PtahBuilder.BuildSystem.Generators
{
    public class OperationProvider<T> : WithOperationContext<T>
    {
        public OperationProvider(IOperationContext<T> context) : base(context)
        {
        }

        public IEnumerable<Operation<T>> BuildOperations()
        {
            yield return new FileMover<T>(Context);

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
