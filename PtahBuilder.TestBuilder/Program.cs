using System;
using System.Linq;
using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Helpers;

namespace PtahBuilder.TestBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger();
            var files = new MinimalConfig.Files();

            var dataGeneratorFactory = new DataGeneratorFactory(logger, files, ReflectionHelper.GetLoadedTypesThatAreAssignableTo(typeof(BaseTypeData)).ToArray());

            dataGeneratorFactory.Process();
        }
    }
}
