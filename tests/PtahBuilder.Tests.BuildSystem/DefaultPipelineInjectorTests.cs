using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Steps.Process;
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
}
