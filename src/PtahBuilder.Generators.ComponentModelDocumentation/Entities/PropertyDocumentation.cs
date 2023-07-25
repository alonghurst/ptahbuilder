using System.Reflection;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Entities;

public record PropertyDocumentation(PropertyInfo PropertyInfo, string DisplayName, string Description);
