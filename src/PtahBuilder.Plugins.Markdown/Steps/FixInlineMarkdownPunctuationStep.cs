using System.Reflection;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.Plugins.Markdown.Steps;

public class FixInlineMarkdownPunctuationStep<T> : IStep<T>
{
    private readonly string _propertyName;

    public FixInlineMarkdownPunctuationStep(string propertyName)
    {
        _propertyName = propertyName;
    }

    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var property = GetWritableStringProperty();

        foreach (var entity in entities)
        {
            var value = property.GetValue(entity.Value) as string;
            property.SetValue(entity.Value, InlineMarkdown.EnsureTerminalPunctuation(value));
        }

        return Task.CompletedTask;
    }

    private PropertyInfo GetWritableStringProperty()
    {
        var property = typeof(T).GetProperty(_propertyName);
        if (property == null)
        {
            throw new InvalidOperationException(
                $"Unable to find a property named \"{_propertyName}\" on type \"{typeof(T).Name}\"");
        }

        if (property.PropertyType != typeof(string))
        {
            throw new InvalidOperationException(
                $"Property \"{_propertyName}\" on type \"{typeof(T).Name}\" is of type {property.PropertyType}");
        }

        if (!property.CanWrite)
        {
            throw new InvalidOperationException(
                $"Property \"{_propertyName}\" on type \"{typeof(T).Name}\" is readonly");
        }

        return property;
    }
}
