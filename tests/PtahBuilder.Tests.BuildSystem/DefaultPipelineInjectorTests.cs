using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Steps.Process;
using PtahBuilder.Util.Services;
using PtahBuilder.Util.Services.Logging;
using Xunit;

namespace PtahBuilder.Tests.BuildSystem;

public class DefaultPipelineInjectorTests
{
    private abstract class BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    private class Apple : BaseEntity
    {
    }

    private class Pie : BaseEntity
    {
    }

    private class Unrelated
    {
    }

    [Fact]
    public void AddDefaultPipelineInjector_AppliesToPipelinesMatchingBaseType()
    {
        var config = CreateConfig();

        config.AddDefaultPipelineInjector<BaseEntity>(p =>
        {
            p.AddProcessStep<FixPunctuationStep<BaseEntity>>(nameof(BaseEntity.Name));
        });

        config.AddPipeline<Apple>(_ => { });
        config.AddPipeline<Pie>(_ => { });
        config.AddPipeline<Unrelated>(_ => { });

        Assert.Single(GetPipeline<Apple>(config).Stages[Stage.Process]);
        Assert.Single(GetPipeline<Pie>(config).Stages[Stage.Process]);
        Assert.Empty(GetPipeline<Unrelated>(config).Stages[Stage.Process]);
    }

    [Fact]
    public void AddDefaultPipelineInjector_SkipsExceptedTypes()
    {
        var config = CreateConfig();

        config.AddDefaultPipelineInjector<BaseEntity>(p =>
        {
            p.AddProcessStep<FixPunctuationStep<BaseEntity>>(nameof(BaseEntity.Name));
        }, except: new[] { typeof(Pie) });

        config.AddPipeline<Apple>(_ => { });
        config.AddPipeline<Pie>(_ => { });

        Assert.Single(GetPipeline<Apple>(config).Stages[Stage.Process]);
        Assert.Empty(GetPipeline<Pie>(config).Stages[Stage.Process]);
    }

    [Fact]
    public void AddDefaultPipelineInjector_AppliesToPipelinesAlreadyConfigured()
    {
        var config = CreateConfig();

        config.AddPipeline<Apple>(_ => { });

        config.AddDefaultPipelineInjector<BaseEntity>(p =>
        {
            p.AddProcessStep<FixPunctuationStep<BaseEntity>>(nameof(BaseEntity.Name));
        });

        Assert.Single(GetPipeline<Apple>(config).Stages[Stage.Process]);
    }

    [Fact]
    public async Task AddDefaultPipelineInjector_RebindsGenericStepsToDerivedType()
    {
        var config = CreateConfig();

        config.AddDefaultPipelineInjector<BaseEntity>(p =>
        {
            p.AddProcessStep<FixPunctuationStep<BaseEntity>>(nameof(BaseEntity.Name));
        });

        config.AddPipeline<Apple>(_ => { });

        var step = GetPipeline<Apple>(config).Stages[Stage.Process][0]
            .CreateStep(new ServiceCollection().BuildServiceProvider());

        Assert.IsType<FixPunctuationStep<Apple>>(step);

        var entity = new Entity<Apple>("apple", new Apple { Name = "Honeycrisp" }, new Metadata());
        await step.Execute(null!, new[] { entity });

        Assert.Equal("Honeycrisp.", entity.Value.Name);
    }

    [Fact]
    public async Task AddDefaultPipelineInjector_CollectsAdaptedEntityAndPipelineValidationErrors()
    {
        var config = CreateConfig();

        config.AddDefaultPipelineInjector<BaseEntity>(p =>
        {
            p.AddProcessStep<RecordValidationStep>();
        });

        config.AddPipeline<Apple>(_ => { });

        var pipeline = new PipelineContext<Apple>(
            config,
            GetPipeline<Apple>(config),
            new NullLogger(),
            new NullDiagnostics());

        var entity = pipeline.AddEntityWithId(new Apple(), "apple");
        var step = GetPipeline<Apple>(config).Stages[Stage.Process][0]
            .CreateStep(new ServiceCollection().BuildServiceProvider());

        await step.Execute(pipeline, new[] { entity });

        var errors = pipeline.ValidationErrors().ToArray();

        var entityErrors = errors.Single(x => x.id == "apple");
        Assert.Equal("entity involved", entityErrors.errors.Single().Error);
        Assert.Contains(nameof(RecordValidationStep), entityErrors.errors.Single().Source);

        var pipelineErrors = errors.Single(x => x.id == PipelineContext<Apple>.PipelineValidationId);
        Assert.Equal("no entity involved", pipelineErrors.errors.Single().Error);
        Assert.Contains(nameof(RecordValidationStep), pipelineErrors.errors.Single().Source);
    }

    [Fact]
    public void AddDefaultPipelineInjector_AppliesWithinPipelinePhases()
    {
        var config = CreateConfig();

        config.AddDefaultPipelineInjector<BaseEntity>(p =>
        {
            p.AddProcessStep<FixPunctuationStep<BaseEntity>>(nameof(BaseEntity.Name));
        }, except: new[] { typeof(Pie) });

        config.AddPipelinePhase(phase =>
        {
            phase.AddPipeline<Apple>(_ => { });
            phase.AddPipeline<Pie>(_ => { });
        });

        Assert.Single(GetPipeline<Apple>(config).Stages[Stage.Process]);
        Assert.Empty(GetPipeline<Pie>(config).Stages[Stage.Process]);
    }

    private static ExecutionConfig CreateConfig() => new(new FilesConfig());

    private static PipelineConfig<T> GetPipeline<T>(ExecutionConfig config) =>
        (PipelineConfig<T>)config.EntityPipelines.Single(x => x.EntityType == typeof(T));

    private class RecordValidationStep : IStep<BaseEntity>
    {
        public Task Execute(IPipelineContext<BaseEntity> context, IReadOnlyCollection<Entity<BaseEntity>> entities)
        {
            context.AddPipelineValidationError(this, "no entity involved");

            foreach (var entity in entities)
            {
                context.AddValidationError(entity, this, "entity involved");
            }

            return Task.CompletedTask;
        }
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
