using System.Reflection;

namespace PtahBuilder.BuildSystem.Services;

public interface IDefaultValueService
{
    /// <summary>
    /// Given an input instance returns any properties which have been modified from the default new() of the instance's type
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    IEnumerable<KeyValuePair<PropertyInfo, object?>> GetNonDefaultPropertyAndTheNewValue(object instance);
}