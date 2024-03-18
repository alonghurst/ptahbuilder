using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Process;

public class ExecuteProcessStep<T> : IStep<T>
{
    private readonly string _executable;
    private readonly string _parametersTemplate;
    private readonly Func<Entity<T>, Dictionary<string, string>> _parametersFactory;

    public ExecuteProcessStep(string executable, string parametersTemplate, Func<Entity<T>, Dictionary<string, string>> parametersFactory)
    {
        _parametersTemplate = parametersTemplate;
        _parametersFactory = parametersFactory;
        _executable = executable;
    }

    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        foreach (var entity in entities)
        {
            var builtParameters = _parametersTemplate;

            var parameters = _parametersFactory(entity);

            foreach (var parameter in parameters)
            {
                builtParameters = builtParameters.Replace(parameter.Key, parameter.Value);
            }

            System.Diagnostics.Process.Start(_executable, builtParameters);
        }

        return Task.CompletedTask;
    }
}