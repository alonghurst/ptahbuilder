using PtahBuilder.BuildSystem;
using Xunit;

namespace PtahBuilder.Tests.BuildSystem;

public class OutputDirectoryTests
{
    [Fact]
    public async Task Run_KeepsOutputDirectoryByDefault()
    {
        var root = Path.Combine(Path.GetTempPath(), "PtahBuilder.OutputDelete", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        try
        {
            var factory = new BuilderFactory()
                .ConfigureFiles(files => files.Configure(root));

            var output = Path.Combine(root, "Output", "stale.json");
            File.WriteAllText(output, "{}");

            await factory.Run();

            Assert.Equal("{}", File.ReadAllText(output));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task Run_DeletesOutputDirectoryWhenRequested()
    {
        var root = Path.Combine(Path.GetTempPath(), "PtahBuilder.OutputDelete", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        try
        {
            var factory = new BuilderFactory()
                .ConfigureFiles(files => files.Configure(root))
                .ConfigureExecution(config => config.DeleteOutputDirectory = true);

            var output = Path.Combine(root, "Output");
            File.WriteAllText(Path.Combine(output, "stale.json"), "{}");

            await factory.Run();

            Assert.False(Directory.Exists(output));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }
}
