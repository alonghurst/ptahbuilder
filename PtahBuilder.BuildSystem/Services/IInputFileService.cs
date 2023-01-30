namespace PtahBuilder.BuildSystem.Services;

public interface IInputFileService
{
    IReadOnlyCollection<string> GetInputFilesForEntity<T>(string fileType);
}