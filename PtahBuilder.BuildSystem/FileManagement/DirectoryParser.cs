using System.IO;

namespace PtahBuilder.BuildSystem.FileManagement;

public abstract class DirectoryParser
{
    public virtual void ParseDirectory(string directoryPath)
    {
        ParseSubDirectory(directoryPath);
    }

    private void ParseSubDirectory(string directoryPath)
    {
        foreach (var directory in Directory.GetDirectories(directoryPath))
        {
            ParseSubDirectory(directory);
        }
        foreach (var filePath in Directory.GetFiles(directoryPath, FileFilter))
        {
            ParseFile(filePath);
        }
    }

    protected abstract string FileFilter { get; }

    protected abstract void ParseFile(string file);
}