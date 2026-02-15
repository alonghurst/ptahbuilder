using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Config;

public class ActivatedStepConfig<T> : IStepConfig<T>
{
    public ActivatedStepConfig(Type stepType, object[] arguments)
    {
        StepType = stepType;
        Arguments = arguments;
    }

    internal Type StepType { get; }

    public object[] Arguments { get; }

    public IStep<T> CreateStep(ServiceProvider serviceProvider) =>
        (IStep<T>)ActivatorUtilities.CreateInstance(serviceProvider, StepType, Arguments);
}
