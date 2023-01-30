using PtahBuilder.BuildSystem.Entities;

namespace PtahBuilder.BuildSystem.Services;

public interface IOutputFileService
{
    string GetOutputDirectoryForEntity<T>();
    string GetOutputFileForEntity<T>(Entity<T> entity, string fileType);
}