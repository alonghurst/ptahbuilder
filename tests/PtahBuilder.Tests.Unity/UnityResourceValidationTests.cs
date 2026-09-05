using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution;
using PtahBuilder.Plugins.Unity.Config;
using PtahBuilder.Plugins.Unity.Validation;
using PtahBuilder.Util.Services;
using PtahBuilder.Util.Services.Logging;
using Xunit;

namespace PtahBuilder.Tests.Unity;

public class UnityResourceValidationTests : IDisposable
{
    private readonly string _root;

    public UnityResourceValidationTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "PtahBuilder.UnityValidation", Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(Path.Combine(_root, "Assets", "Resources", "Prefabs"));
        File.WriteAllText(Path.Combine(_root, "Assets", "Resources", "Prefabs", "Goblin.prefab"), string.Empty);

        Directory.CreateDirectory(Path.Combine(_root, "Assets", "Resources", "Prefabs", "Orc"));
        File.WriteAllText(Path.Combine(_root, "Assets", "Resources", "Prefabs", "Orc", "Orc.prefab"), string.Empty);

        Directory.CreateDirectory(Path.Combine(_root, "Assets", "Resources", "Prefabs", "Enemies"));
        File.WriteAllText(Path.Combine(_root, "Assets", "Resources", "Prefabs", "Enemies", "Dragon.prefab"), string.Empty);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, recursive: true);
    }

    [Fact]
    public void Exists_WhenSubfoldersAreNotConsidered_FindsTopLevelAndNamedFolderLayoutsOnly()
    {
        var basePath = Path.Combine(_root, "Assets", "Resources", "Prefabs");

        Assert.True(UnityResourceFiles.Exists(basePath, "Goblin", "prefab", considerSubfolders: false));
        Assert.True(UnityResourceFiles.Exists(basePath, "Orc", "prefab", considerSubfolders: false));
        Assert.False(UnityResourceFiles.Exists(basePath, "Dragon", "prefab", considerSubfolders: false));
    }

    [Fact]
    public void Exists_WhenSubfoldersAreConsidered_FindsFilesInNestedFolders()
    {
        var basePath = Path.Combine(_root, "Assets", "Resources", "Prefabs");

        Assert.True(UnityResourceFiles.Exists(basePath, "Dragon", "prefab", considerSubfolders: true));
    }

    [Fact]
    public async Task ValidateUnityResourceStep_ReportsMissingNestedResourceUnlessSubfoldersAreConsidered()
    {
        var unity = new UnityConfig { ProjectDirectory = _root };
        var context = CreateContext();
        var dragon = context.AddEntityWithId(new Item(), "Dragon");
        var goblin = context.AddEntityWithId(new Item(), "Goblin");

        await new ValidateUnityResourceStep<Item>(unity, "Prefabs", "prefab")
            .Execute(context, context.Entities.Values.ToArray());

        Assert.Contains(dragon.Validation.Errors, e => e.Error.Contains("Dragon.prefab"));
        Assert.Empty(goblin.Validation.Errors);

        var nestedContext = CreateContext();
        var nestedDragon = nestedContext.AddEntityWithId(new Item(), "Dragon");

        await new ValidateUnityResourceStep<Item>(unity, "Prefabs", "prefab", considerSubfolders: true)
            .Execute(nestedContext, nestedContext.Entities.Values.ToArray());

        Assert.Empty(nestedDragon.Validation.Errors);
    }

    [Fact]
    public async Task AddProcessStep_BindsConsiderSubfoldersFromBooleanArgument()
    {
        var services = new ServiceCollection()
            .AddSingleton(new UnityConfig { ProjectDirectory = _root })
            .BuildServiceProvider();

        var step = new ActivatedStepConfig<Item>(
                typeof(ValidateUnityResourceStep<Item>),
                ["Prefabs", "prefab", true])
            .CreateStep(services);

        var context = CreateContext();
        var dragon = context.AddEntityWithId(new Item(), "Dragon");

        await step.Execute(context, context.Entities.Values.ToArray());

        Assert.Empty(dragon.Validation.Errors);
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
