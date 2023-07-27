namespace PtahBuilder.Generators.ComponentModelDocumentation.Entities;

internal record TypeDocumentation
(
    Type Type,
    string DisplayName,
    string Description,
    IReadOnlyCollection<PropertyDocumentation> Properties,
    IReadOnlyCollection<EnumValueDocumentation> EnumValues,
    ObsoleteDocumentation? Obsolete = null
);
