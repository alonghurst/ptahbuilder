using System.Reflection;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Entities;

internal record PropertyDocumentation(PropertyInfo PropertyInfo, string DisplayName, string Description);
