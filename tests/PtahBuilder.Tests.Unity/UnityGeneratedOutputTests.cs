using PtahBuilder.BuildSystem;
using PtahBuilder.Plugins.Unity;
using PtahBuilder.Plugins.Unity.Extensions;
using Xunit;

namespace PtahBuilder.Tests.Unity;

public class UnityGeneratedOutputTests : IDisposable
{
    private readonly string _root;

    public UnityGeneratedOutputTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "PtahBuilder.UnityOutput", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, recursive: true);
    }

    [Fact]
    public void RemoveStale_KeepsRewrittenJsonAndItsMeta()
    {
        var cutoff = new DateTime(2026, 11, 14, 12, 0, 0, DateTimeKind.Utc);
        var json = Path.Combine(_root, "Enemies", "Ram.json");
        var meta = json + ".meta";
        var folderMeta = Path.Combine(_root, "Enemies.meta");
        Write(json, "{}", cutoff.AddMinutes(1));
        Write(meta, "guid: ram", cutoff.AddDays(-1));
        Write(folderMeta, "guid: enemies", cutoff.AddDays(-1));

        UnityGeneratedOutput.RemoveStale(_root, cutoff);

        Assert.True(File.Exists(json));
        Assert.Equal("guid: ram", File.ReadAllText(meta));
        Assert.Equal("guid: enemies", File.ReadAllText(folderMeta));
        Assert.True(Directory.Exists(Path.Combine(_root, "Enemies")));
    }

    [Fact]
    public void RemoveStale_DropsOldJsonItsMetaAndAnEmptyFolder()
    {
        var cutoff = new DateTime(2026, 11, 14, 12, 0, 0, DateTimeKind.Utc);
        var oldJson = Path.Combine(_root, "Gone", "Old.json");
        var kept = Path.Combine(_root, "notes.txt");
        Write(oldJson, "{}", cutoff.AddMinutes(-1));
        Write(oldJson + ".meta", "guid: old", cutoff.AddDays(-2));
        Write(Path.Combine(_root, "Gone.meta"), "guid: gone", cutoff.AddDays(-2));
        Write(kept, "leave", cutoff.AddDays(-3));

        UnityGeneratedOutput.RemoveStale(_root, cutoff);

        Assert.False(File.Exists(oldJson));
        Assert.False(File.Exists(oldJson + ".meta"));
        Assert.False(Directory.Exists(Path.Combine(_root, "Gone")));
        Assert.False(File.Exists(Path.Combine(_root, "Gone.meta")));
        Assert.Equal("leave", File.ReadAllText(kept));
    }

    [Fact]
    public async Task UseUnity_RewritesJsonAndKeepsMetaGuids()
    {
        var project = UnityProject();
        var output = Path.Combine(project, "Assets", "Resources", "Data");
        var enemies = Path.Combine(output, "Enemies");
        var oldJson = Path.Combine(enemies, "Old.json");
        var keptJson = Path.Combine(enemies, "Kept.json");
        Write(oldJson, "{}", DateTime.UtcNow.AddHours(-1));
        Write(oldJson + ".meta", "guid: old", DateTime.UtcNow.AddDays(-2));
        Write(keptJson, "{}", DateTime.UtcNow.AddHours(-1));
        Write(keptJson + ".meta", "guid: kept", DateTime.UtcNow.AddDays(-2));
        Write(Path.Combine(output, "Enemies.meta"), "guid: enemies", DateTime.UtcNow.AddDays(-2));

        await new BuilderFactory()
            .UseUnity(_root)
            .ConfigureExecution(config =>
            {
                config.PreExecution = _ => File.WriteAllText(keptJson, "{\"id\":\"kept\"}");
            })
            .Run();

        Assert.False(File.Exists(oldJson));
        Assert.False(File.Exists(oldJson + ".meta"));
        Assert.Equal("{\"id\":\"kept\"}", File.ReadAllText(keptJson));
        Assert.Equal("guid: kept", File.ReadAllText(keptJson + ".meta"));
        Assert.Equal("guid: enemies", File.ReadAllText(Path.Combine(output, "Enemies.meta")));
    }

    [Fact]
    public async Task UseUnity_DeleteOutputDirectory_WipesTheOutputFolder()
    {
        var project = UnityProject();
        var output = Path.Combine(project, "Assets", "Resources", "Data");
        var json = Path.Combine(output, "Enemies", "Old.json");
        Write(json, "{}", DateTime.UtcNow.AddHours(-1));
        Write(json + ".meta", "guid: old", DateTime.UtcNow.AddDays(-2));

        await new BuilderFactory()
            .UseUnity(_root)
            .ConfigureExecution(config => config.DeleteOutputDirectory = true)
            .Run();

        Assert.False(Directory.Exists(output));
    }

    private string UnityProject()
    {
        var name = Path.GetFileName(_root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var project = Path.Combine(_root, name + ".Unity");
        Directory.CreateDirectory(project);
        return project;
    }

    private static void Write(string path, string contents, DateTime writtenAtUtc)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, contents);
        File.SetLastWriteTimeUtc(path, writtenAtUtc);
    }
}
