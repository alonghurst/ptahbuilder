using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PtahBuilder.BuildSystem.Generators.Operations
{
    public class InstanceToTypeFactoryDefinitionsOperationProvider<T> : OperationProvider<T>
    {
        protected virtual Operation<T> FinalOperation()
        {
            return new InstanceToTypeFactoryDefinitionsOperation<T>(Context);
        }
    }
}
