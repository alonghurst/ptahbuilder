namespace PtahBuilder.Generators.ComponentModelDocumentation.Entities;

public record EnumValueDocumentation(string Id, string DisplayName, string Description, ObsoleteDocumentation? Obsolete = null);