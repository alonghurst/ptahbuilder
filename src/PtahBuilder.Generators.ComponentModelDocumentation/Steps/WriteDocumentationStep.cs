using System.Text.RegularExpressions;
using Grynwald.MarkdownGenerator;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Steps.Output.SimpleText;
using PtahBuilder.Generators.ComponentModelDocumentation.Entities;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Steps;

internal class WriteDocumentationStep : IStep<SimpleTextOutput>
{
    private readonly IEntityProvider<TypeDocumentation> _entityProvider;
    private readonly IFilesConfig _files;

    public WriteDocumentationStep(IEntityProvider<TypeDocumentation> entityProvider, IFilesConfig files)
    {
        _entityProvider = entityProvider;
        _files = files;
    }

    // TODO: get path from somewhere configurable
    private string Directory() => Path.Combine(_files.WorkingDirectory, "Docs");

    public Task Execute(IPipelineContext<SimpleTextOutput> context, IReadOnlyCollection<Entity<SimpleTextOutput>> entities)
    {
        var documentationTypes = _entityProvider.Entities;

        foreach (var entity in documentationTypes)
        {
            var file = entity.Value.Id;
            var contents = MakeContents(entity.Value.Value, documentationTypes);

            var simpleText = new SimpleTextOutput(file, "md", Directory(), contents);

            context.AddEntity(simpleText);
        }

        var index = MakeIndex(_entityProvider.Entities.Values);
        context.AddEntity(index);

        return Task.CompletedTask;
    }

    private SimpleTextOutput MakeIndex(Dictionary<string, Entity<TypeDocumentation>>.ValueCollection values)
    {
        var entities = values.Select(x => x.Value)
            .OrderBy(x => x.DisplayName)
            .ToArray();

        var markdown = new MdDocument();

        markdown.Root.Add(new MdHeading("Index", 1));

        string DisplayName(TypeDocumentation x)
        {
            var obsolete = x.Obsolete != null ? " (Obsolete)" : string.Empty;
            var display = $"{x.DisplayName}{obsolete}";

            return display;
        }

        var links = entities
            .Select(x => new MdLinkSpan(DisplayName(x), x.DisplayName.MakeUri()))
            .Select(x => new MdListItem(x));

        markdown.Root.Add(new MdBulletList(links));

        return new("Index", "md", Directory(), markdown.ToString());
    }

    private string MakeContents(TypeDocumentation documentation, Dictionary<string, Entity<TypeDocumentation>> documentationTypes)
    {
        IEnumerable<MdBlock> ProcessTextBlock(string text)
        {
            var parts = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            foreach (var para in parts)
            {
                yield return new MdParagraph(ProcessText(para).ToArray());
            }
        }

        MdBlock[] Describe(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return new MdBlock[] { new MdParagraph(new MdEmphasisSpan("No documentation available.")) };
            }

            return ProcessTextBlock(str).ToArray();
        }

        MdBlock[] DescribeObsolete(ObsoleteDocumentation? obsoleteDocumentation)
        {
            if (obsoleteDocumentation != null)
            {
                return new[]
                {
                    new MdParagraph(new MdStrongEmphasisSpan("Obsolete:"), new MdTextSpan(" This entity has been marked as obsolete and should not be used. "), new MdTextSpan(obsoleteDocumentation.Description))
                };
            }

            return Array.Empty<MdBlock>();
        }

        var markdown = new MdDocument();

        markdown.Root.Add(new MdHeading(documentation.DisplayName, 1));
        markdown.Root.Add(DescribeObsolete(documentation.Obsolete));
        markdown.Root.Add(Describe(documentation.Description));

        if (documentation.Properties.Any())
        {
            markdown.Root.Add(new MdHeading(2, "Properties"));

            foreach (var property in documentation.Properties)
            {
                markdown.Root.Add(new MdHeading(3, property.DisplayName));

                MdSpan typeDescription = $"Type: {property.PropertyInfo.PropertyType.GetTypeName()}";

                var type = documentationTypes.Values.FirstOrDefault(x => x.Value.Type == property.PropertyInfo.PropertyType);

                if (type != null)
                {
                    typeDescription = new MdCompositeSpan(typeDescription, " (", new MdLinkSpan(type.Value.DisplayName, type.Id.MakeUri()), ")");
                }

                markdown.Root.Add(new MdParagraph(typeDescription));
                markdown.Root.Add(DescribeObsolete(property.Obsolete));
                markdown.Root.Add(Describe(property.Description));
            }
        }

        if (documentation.EnumValues.Any())
        {
            markdown.Root.Add(new MdHeading(2, "Values"));

            foreach (var enumValue in documentation.EnumValues)
            {
                markdown.Root.Add(new MdHeading(3, enumValue.DisplayName));
                markdown.Root.Add(DescribeObsolete(enumValue.Obsolete));
                markdown.Root.Add(Describe(enumValue.Description));
            }
        }

        return markdown.ToString();
    }

    private IEnumerable<MdSpan> ProcessText(string text)
    {
        var parts = Regex.Split(text, ("( )"));

        foreach (var part in parts)
        {
            if (part.StartsWith("#"))
            {
                var l = part.Substring(1);
                yield return new MdLinkSpan(l, l.MakeUri());
            }
            else
            {
                yield return part;
            }
        }
    }
}

internal static class Extensions
{
    internal static string MakeUri(this string uri) => $"{uri}.md";
}