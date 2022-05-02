using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.FileManagement;

public class PathResolver
{
    public IFiles Files { get; }

    public PathResolver(IFiles files)
    {
        Files = files;
    }

    public string MetaDirectory(params string[] directories)
    {
        if (directories == null || directories.Length == 0)
        {
            return DataDirectory("Meta");
        }

        var str = new[] { DataDirectory("Meta") }.Union(directories).ToArray();
        return Path.Combine(str);
    }

    public string MetaFile(string fileName, string extension)
    {
        var meta = MetaDirectory();

        if (!Directory.Exists(meta))
        {
            Directory.CreateDirectory(meta);
        }

        return Path.Combine(meta, $"{fileName}.{extension}");
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

    public string OutputFile(string fileName, string extension)
    {
        var directoryPath = Files.Output;

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        return Path.Combine(directoryPath, fileName + extension);
    }

    public string OutputCodeFile(string directory, string fileName)
    {
        var directoryPath = Path.Combine(Files.Output, directory);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        return Path.Combine(directoryPath, fileName + ".Generated.cs");
    }

    public string FactoryOutputFile<T>(BaseDataMetadataResolver<T> metadataResolver, string fileType)
    {
        return OutputCodeFile("Factories", $"Factory.{metadataResolver.DataDirectoryToOperateIn}.{fileType}");
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

    public string GetYamlFromDataFiles(string[] directories, string fileNameWithoutExtension)
    {
        var directory = DataDirectory(directories);

        var fileName = new FileInfo(Path.Combine(directory, fileNameWithoutExtension + ".yml"));

        if (fileName.Exists)
        {
            return fileName.FullName;
        }

        fileName = new FileInfo(Path.Combine(directory, fileNameWithoutExtension + ".yaml"));

        if (fileName.Exists)
        {
            return fileName.FullName;
        }

        return null;
    }
}