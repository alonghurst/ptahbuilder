using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Extensions
{
    public static class IEntityProviderExtensions
    {
        public static T[] ToArray<T>(this IEntityProvider<T> provider) => provider.Entities.Values.Select(x => x.Value).ToArray();
    }
}
