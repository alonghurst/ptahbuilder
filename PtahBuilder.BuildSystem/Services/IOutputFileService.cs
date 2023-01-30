using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution;

namespace PtahBuilder.BuildSystem.Services;

public interface IOutputFileService
{
    string GetOutputDirectoryForEntity<T>();
    string GetOutputFileForEntity<T>(Entity<T> entity, string fileType);
}