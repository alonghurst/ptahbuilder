using PtahBuilder.LegacyBuildSystem;
using PtahBuilder.Tests.LegacyTestBuilder.Types;
using PtahBuilder.Util.Helpers;

namespace PtahBuilder.Tests.LegacyTestBuilder;

class Program
{
    // ReSharper disable once UnusedParameter.Local
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            MinimalConfig.Files.ForcedRoot = args[0];
        }

        var logger = new Logger();
        var files = new MinimalConfig.Files();

        var dataGeneratorFactory = new DataGeneratorFactory(logger, files, ReflectionHelper.GetLoadedTypesThatAreAssignableTo(typeof(BaseTypeData))
            .Except(new[] { typeof(BaseTypeData) })
            .ToArray());

        dataGeneratorFactory.Process();

        logger.ToReportHtml();
    }
}