using System.Reflection;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Process;

public class FixPunctuationStep<T> : IStep<T>
{
    private readonly string _propertyName;

    private readonly string[] _validEndings =
    {
        ".", "!", "?", ">", "]", "}"
    };

    public FixPunctuationStep(string propertyName)
    {
        _propertyName = propertyName;
    }

    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var properties = typeof(T).GetProperties()
            .Where(x => IsToBeFixed(x))
            .ToArray();

        foreach (var entity in entities)
        {
            foreach (var property in properties)
            {
                var value = property.GetValue(entity.Value);

                if (value is string[] text)
                {
                    text = text.Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    if (text.Length == 0)
                    {
                        text = Array.Empty<string>();
                    }
                    else
                    {
                        for (int i = 0; i < text.Length; i++)
                        {
                            text[i] = Fix(text[i]);
                        }
                    }

                    property.SetValue(entity.Value, text);
                }
                else if (value is string str)
                {
                    property.SetValue(entity.Value, Fix(str));
                }
            }
        }

        return Task.CompletedTask;
    }

    protected virtual bool IsToBeFixed(PropertyInfo propertyInfo)
    {
        if (_propertyName == propertyInfo.Name)
        {
            return true;
        }

        return false;
    }

    private string Fix(string text)
    {
        //text = text.Trim();

        foreach (var validEnding in _validEndings)
        {
            if (text.EndsWith(validEnding))
            {
                return text;
            }
        }

        return $"{text}.";
    }
}