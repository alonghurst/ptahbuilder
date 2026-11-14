using PtahBuilder.Util.Services.Logging;
using Xunit;

namespace PtahBuilder.Tests.BuildSystem;

public class FileLoggerTests
{
    [Fact]
    public void TwoLoggers_OpenedInTheSameSecond_BothWrite()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PtahBuilder.Logs", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        try
        {
            using (var first = new FileLogger(directory))
            using (var second = new FileLogger(directory))
            {
                first.Info("one");
                second.Info("two");
            }

            var content = string.Concat(Directory.GetFiles(directory, "log_*.txt").Select(File.ReadAllText));
            Assert.Contains("one", content);
            Assert.Contains("two", content);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}
