using System.Reflection;
using PtahBuilder.Generators.ComponentModelDocumentation.Entities;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Services
{
    public class ObsoleteDocumentationService : IObsoleteDocumentationService
    {
        public ObsoleteDocumentation? TypeObsoleteDocumentation(Type type)
        {
            var attribute = type.GetCustomAttribute<ObsoleteAttribute>();

            return DocumentAttribute(attribute);
        }


        public ObsoleteDocumentation? EnumValueObsoleteDocumentation(Type type, object value)
        {
            var attribute = type.GetCustomAttributeForEnumValue<ObsoleteAttribute>(value);

            return DocumentAttribute(attribute);
        }

        public ObsoleteDocumentation? PropertyObsoleteDocumentation(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<ObsoleteAttribute>();

            return DocumentAttribute(attribute);
        }

        private ObsoleteDocumentation? DocumentAttribute(ObsoleteAttribute? attribute)
        {
            if (attribute == null)
            {
                return null;
            }

            return new ObsoleteDocumentation(attribute.Message ?? string.Empty);
        }
    }
}
