using System.Collections.Generic;
using System.IO;
using System.Linq;
using PtahBuilder.BuildSystem.Metadata;
using PtahBuilder.BuildSystem.Operations;

namespace PtahBuilder.BuildSystem.FileManagement
{
    public class FileMover<T> : IOperation<T> where T : TypeData
    {
        public PathResolver PathResolver { get; }
        public Logger Logger { get; }
        public BaseDataMetadataResolver<T> MetadataResolver { get; }

        public FileMover(Logger logger, PathResolver pathResolver, BaseDataMetadataResolver<T> metadataResolver)
        {
            MetadataResolver = metadataResolver;
            Logger = logger;
            PathResolver = pathResolver;
        }


        public Dictionary<T, MetadataCollection> Operate(Dictionary<T, MetadataCollection> entities)
        {
            var files = Directory.GetFiles(PathResolver.DataDirectory(MetadataResolver.DataDirectoryToOperateIn), "*.yaml");

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                var typeEntity = entities.Keys.FirstOrDefault(i => i.TypeName?.ToLower() == name.ToLower());

                if (typeEntity == null)
                {
                    Logger.Warning($"Unable to find corresponding {MetadataResolver.EntityName} yaml file for {name}");
                }
                else
                {
                    var category = MetadataResolver.GetEntityCategory(typeEntity);

                    if (string.IsNullOrEmpty(category))
                    {
                        continue;
                    }

                    var targetDirectory = PathResolver.DataDirectory(MetadataResolver.DataDirectoryToOperateIn, category);
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    var old = fileInfo.FullName;
                    var newF = Path.Combine(targetDirectory, fileInfo.Name);

                    fileInfo.MoveTo(newF);

                    Logger.LogSection("Moved yaml Files", $"{old} -> {newF}");
                }
            }

            return entities;
        }
    }
}
