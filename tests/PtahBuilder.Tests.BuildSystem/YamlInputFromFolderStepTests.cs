using PtahBuilder.BuildSystem.Steps.Input;
using Xunit;

namespace PtahBuilder.Tests.BuildSystem;

public class YamlInputFromFolderStepTests
{
    private class SampleEntity
    {
    }

    [Fact]
    public void ListYamlFiles_ReadsNestedFilesUnderFolder()
    {
        var root = Path.Combine(Path.GetTempPath(), "ptah-yaml-" + Guid.NewGuid().ToString("N"));
        var nested = Path.Combine(root, "Extra", "Group");
        Directory.CreateDirectory(nested);
        var file = Path.Combine(nested, "Item.yaml");
        File.WriteAllText(file, "Id: Item");

        try
        {
            var files = YamlInputFromFolderStep<SampleEntity>.ListYamlFiles(root, "Extra");

            Assert.Equal(file, Assert.Single(files));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ListYamlFiles_WhenFolderMissing_ReturnsNone()
    {
        var root = Path.Combine(Path.GetTempPath(), "ptah-yaml-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        try
        {
            Assert.Empty(YamlInputFromFolderStep<SampleEntity>.ListYamlFiles(root, "Extra"));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
