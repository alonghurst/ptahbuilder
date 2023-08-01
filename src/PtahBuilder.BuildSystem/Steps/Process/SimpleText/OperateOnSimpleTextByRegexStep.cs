using System.Text.RegularExpressions;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Steps.Input.SimpleText;
using PtahBuilder.BuildSystem.Steps.Output.SimpleText;

namespace PtahBuilder.BuildSystem.Steps.Process.SimpleText;

public abstract class OperateOnSimpleTextByRegexStep : IStep<SimpleTextInput>
{
    private readonly Regex[] _regexes;

    protected OperateOnSimpleTextByRegexStep(params string[] regexes)
    {
        _regexes = regexes.Select(x=> new Regex(x)).ToArray();
    }

    public Task Execute(IPipelineContext<SimpleTextInput> context, IReadOnlyCollection<Entity<SimpleTextInput>> entities)
    {
        foreach (var entity in entities)
        {
            foreach (var regex in _regexes)
            {
                var matches = regex.Matches(entity.Value.Contents);

                foreach (Match match in matches)
                {
                    OperateOnMatch(context, entity, match);
                }
            }
        }

        return Task.CompletedTask;
    }

    protected abstract void OperateOnMatch(IPipelineContext<SimpleTextInput> context, Entity<SimpleTextInput> entity, Match match);
}