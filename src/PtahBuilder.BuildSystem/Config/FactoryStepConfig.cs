using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Config;

public class FactoryStepConfig<T> : IStepConfig<T>
{
    public FactoryStepConfig(Func<ServiceProvider, IStep<T>> factory)
    {
        Factory = factory;
    }

    public Func<ServiceProvider, IStep<T>> Factory { get; }

    public IStep<T> CreateStep(ServiceProvider serviceProvider) => Factory(serviceProvider);
}
