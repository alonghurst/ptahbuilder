using PtahBuilder.BuildSystem.Entities;

namespace PtahBuilder.BuildSystem.Services.Files;

public interface IOutputFileService
{
    string GetOutputDirectoryForEntity<T>();
    string GetOutputFileForEntity<T>(Entity<T> entity, string fileType);
}