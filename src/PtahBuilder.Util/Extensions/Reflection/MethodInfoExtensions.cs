using System.Reflection;

namespace PtahBuilder.Util.Extensions.Reflection;

public static class MethodInfoExtensions
{
#nullable disable
    public static async Task<object> InvokeAsync(this MethodInfo method, object obj, params object[] parameters)
    {
        var task = (Task)method.Invoke(obj, parameters);
        await task.ConfigureAwait(false);
        var resultProperty = task.GetType().GetProperty("Result");
        return resultProperty?.GetValue(task);
    }
#nullable enable
}