using System.IO;
using System.Linq;
using PtahBuilder.BuildSystem.Generators.Context;

namespace PtahBuilder.BuildSystem.Generators.Operations
{
    public class FileMover<T> : Operation<T>
    {
        public FileMover(IOperationContext<T> context) : base(context)
        {
        }

        [Operate]
        public void Operate()
        {
            var files = Directory.GetFiles(PathResolver.DataDirectory(MetadataResolver.DataDirectoryToOperateIn), "*.yaml");

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                var typeEntity = Entities.Keys.FirstOrDefault(i => MetadataResolver.GetEntityId(i).ToLower() == name.ToLower());

                if (typeEntity == null)
                {
                    Logger.Warning($"Unable to find corresponding {MetadataResolver.EntityTypeName} yaml file for {name}");
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
        }
    }
}
