using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Config
{
    public class DocumentationConfig
    {
        private readonly List<Type> _typesToDocument  = new List<Type>();

        public DocumentationConfig AddType(Type type)
        {
            _typesToDocument.Add(type);
            return this;
        }

        public DocumentationConfig AddTypesInheritedFrom(Type type, bool includeAbstract = false)
        {
            var types = ReflectionExtensions.GetLoadedTypesThatAreAssignableTo(type, !includeAbstract);

            foreach (var childType in types)
            {
                AddType(childType);
            }

            return this;
        }

        internal IReadOnlyCollection<Type> GetTypesToDocument() => _typesToDocument.Distinct().ToArray();
    }
}
