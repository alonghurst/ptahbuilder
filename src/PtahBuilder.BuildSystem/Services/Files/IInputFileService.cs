namespace PtahBuilder.BuildSystem.Services.Files;

public interface IInputFileService
{
    IReadOnlyCollection<string> GetInputFilesForEntity<T>(string fileType);
}