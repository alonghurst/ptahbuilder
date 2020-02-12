﻿using System.IO;
using System.Linq;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.FileManagement
{
    public class PathResolver
    {
        public IFiles Files { get; }

        public PathResolver(IFiles files)
        {
            Files = files;
        }

        public string DataDirectory(params string[] directories)
        {
            if (directories == null || directories.Length == 0)
            {
                return Path.Combine(Files.Root, "Data");
            }

            var str = new[] { Files.Root, "Data" }.Union(directories).ToArray();
            return Path.Combine(str);
        }

        public string DataFile(string directory, string fileName)
        {
            return Path.Combine(DataDirectory(directory), fileName + ".yaml");
        }

        public string OutputFile(string directory, string fileName)
        {
            var directoryPath = Path.Combine(Files.OutputForCode, directory);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return Path.Combine(directoryPath, fileName + ".Generated.cs");
        }

        public string FactoryOutputFile<T>(BaseDataMetadataResolver<T> metadataResolver, string fileType) where T : TypeData
        {
            return OutputFile("Factories", $"Factory.{metadataResolver.DataDirectoryToOperateIn}.{fileType}");
        }

        public string FindDataFile(string inFolder, string typeName)
        {
            var dataDirectory = DataDirectory(inFolder);
            return Find(dataDirectory, typeName);
        }

        private string Find(string directory, string typeName)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (Path.GetFileNameWithoutExtension(file) == typeName)
                {
                    return file;
                }
            }

            foreach (var director in Directory.GetDirectories(directory))
            {
                var file = Find(director, typeName);
                if (!string.IsNullOrEmpty(file))
                {
                    return file;
                }
            }


            return null;
        }
    }
}
