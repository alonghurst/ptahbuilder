using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Config;

public interface IStepConfig<T>
{
    IStep<T> CreateStep(ServiceProvider serviceProvider);
}
