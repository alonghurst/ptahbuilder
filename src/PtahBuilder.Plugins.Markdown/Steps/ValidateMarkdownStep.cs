using System.Reflection;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.Plugins.Markdown.Steps;

public class ValidateMarkdownStep<T> : IStep<T>
{
    private readonly string _propertyName;

    public ValidateMarkdownStep(string propertyName)
    {
        _propertyName = propertyName;
    }

    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var property = GetStringProperty();

        foreach (var entity in entities)
        {
            var value = property.GetValue(entity.Value) as string;
            foreach (var error in InlineMarkdown.Validate(value))
            {
                context.AddValidationError(
                    entity,
                    this,
                    $"{typeof(T).Name} '{entity.Id}' {_propertyName}: {error}");
            }
        }

        return Task.CompletedTask;
    }

    private PropertyInfo GetStringProperty()
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

        return property;
    }
}
