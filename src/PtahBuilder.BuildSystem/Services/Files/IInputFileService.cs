namespace PtahBuilder.BuildSystem.Services.Files;

public interface IInputFileService
{
    IReadOnlyCollection<string> GetInputFilesForEntityType<T>(string fileType);
    string GetInputDirectoryForEntityType<T>();
}