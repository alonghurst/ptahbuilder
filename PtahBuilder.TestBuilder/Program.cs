using System.Linq;
using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Helpers;
using PtahBuilder.TestBuilder.Types;

namespace PtahBuilder.TestBuilder
{
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
}
