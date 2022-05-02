using System.Reflection;

namespace PtahBuilder.BuildSystem.Validation;

public class StringParameterVerification<T>
{
    private readonly PropertyInfo[] _properties;
    private readonly Func<T, string> _descriptorFunc;

    public StringParameterVerification(Func<T, string> descriptorFunc, params string[] properties)
    {
        _properties = typeof(T).GetProperties().Where(t => properties.Contains(t.Name)).ToArray();
        _descriptorFunc = descriptorFunc;
    }

    public IEnumerable<KeyValuePair<string, PropertyInfo>> Validate(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            foreach (var property in _properties)
            {
                var value = property.GetValue(entity);

                if (string.IsNullOrEmpty(value?.ToString()))
                {
                    yield return new KeyValuePair<string, PropertyInfo>(_descriptorFunc(entity), property);
                }
            }
        }
    }
}