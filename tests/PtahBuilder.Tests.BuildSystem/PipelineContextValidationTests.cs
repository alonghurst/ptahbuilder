using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Util.Services;
using PtahBuilder.Util.Services.Logging;
using Xunit;

namespace PtahBuilder.Tests.BuildSystem;

public class PipelineContextValidationTests
{
    [Fact]
    public void ValidationErrors_IncludesEntityAndPipelineErrors()
    {
        var context = CreateContext();
        var entity = context.AddEntityWithId(new Item(), "apple");

        context.AddValidationError(entity, "EntityStep", "entity is invalid");
        context.AddPipelineValidationError("PipelineStep", "pipeline is invalid");

        var errors = context.ValidationErrors().ToArray();

        Assert.Equal(2, errors.Length);

        var entityErrors = errors.Single(x => x.id == "apple");
        Assert.Equal(typeof(Item), entityErrors.type);
        Assert.Equal("EntityStep", entityErrors.errors.Single().Source);
        Assert.Equal("entity is invalid", entityErrors.errors.Single().Error);

        var pipelineErrors = errors.Single(x => x.id == PipelineContext<Item>.PipelineValidationId);
        Assert.Equal(typeof(Item), pipelineErrors.type);
        Assert.Equal("PipelineStep", pipelineErrors.errors.Single().Source);
        Assert.Equal("pipeline is invalid", pipelineErrors.errors.Single().Error);
    }

    [Fact]
    public void ValidationErrors_KeepsPipelineErrorsAfterEntityIsRemoved()
    {
        var context = CreateContext();
        var entity = context.AddEntityWithId(new Item(), "apple");

        context.AddValidationError(entity, "EntityStep", "entity is invalid");
        context.AddPipelineValidationError("PipelineStep", "pipeline is invalid");
        context.RemoveEntity(entity);

        var errors = context.ValidationErrors().ToArray();

        Assert.Contains(errors, x => x.id == "apple" && x.errors.Single().Error == "entity is invalid");
        Assert.Contains(errors, x => x.id == PipelineContext<Item>.PipelineValidationId && x.errors.Single().Error == "pipeline is invalid");
    }

    private static PipelineContext<Item> CreateContext() =>
        new(
            new ExecutionConfig(new FilesConfig()),
            new PipelineConfig<Item>("Item_Pipeline"),
            new NullLogger(),
            new NullDiagnostics());

    private sealed class Item
    {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class NullLogger : ILogger
    {
        public void Info(string message) { }
        public void Warning(string message) { }
        public void Error(string message) { }
        public void Success(string message) { }
        public void Verbose(string message) { }
    }

    private sealed class NullDiagnostics : IDiagnostics
    {
        public void Time(string operationDescription, Action operation) => operation();
        public T Time<T>(string operationDescription, Func<T> operation) => operation();
        public Task Time(string operationDescription, Func<Task> operation) => operation();
    }
}
