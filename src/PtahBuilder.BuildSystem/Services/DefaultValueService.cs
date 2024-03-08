using System.Collections;
using System.Reflection;

namespace PtahBuilder.BuildSystem.Services;

public class DefaultValueService : IDefaultValueService
{
    private readonly Dictionary<Type, object> _blankInstances = new Dictionary<Type, object>();

    public IEnumerable<KeyValuePair<PropertyInfo, object>> GetNonDefaultPropertyAndTheNewValue(object instance)
    {
        var type = instance.GetType();
        if (!_blankInstances.ContainsKey(type))
        {
            var blank = Activator.CreateInstance(type);

            if (blank == null)
            {
                throw new InvalidOperationException($"Unable to instantiate a {type.Name}");
            }

            _blankInstances.Add(type, blank);
        }

        var blankInstance = _blankInstances[type];

        foreach (var property in type.GetProperties().Where(p => p.CanWrite))
        {
            var a = property.GetValue(instance);
            var b = property.GetValue(blankInstance);

            if (a == null && b == null)
            {
                continue;
            }

            if (a != null && b == null)
            {
                yield return new KeyValuePair<PropertyInfo, object>(property, a);
                continue;
            }

            var propertyType = property.PropertyType;
            if (propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                dynamic aEnumerable = a ?? Array.CreateInstance(propertyType, 0);
                // ReSharper disable once ConstantNullCoalescingCondition
                dynamic bEnumerable = b ?? Array.CreateInstance(propertyType, 0);

                var equal = true;

                foreach (var ae in aEnumerable)
                {
                    equal = false;
                    foreach (var be in bEnumerable)
                    {
                        if (ae == be)
                        {
                            equal = true;
                            break;
                        }
                    }

                    if (!equal)
                    {
                        break;
                    }
                }

                if (equal)
                {
                    continue;
                }
            }

            if (a == null || !a.Equals(b))
            {
                yield return new KeyValuePair<PropertyInfo, object>(property, a);
            }
        }
    }
}