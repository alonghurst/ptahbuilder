namespace PtahBuilder.Plugins.Unity.Config;

/// <summary>
/// Configuration for the Unity project used by the builder (e.g. asset paths, project root).
/// </summary>
public class UnityConfig
{
    /// <summary>
    /// Full path to the Unity project directory (e.g. the folder containing Assets, ProjectSettings).
    /// </summary>
    public string ProjectDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Path to the Unity project's Assets folder (ProjectDirectory/Assets).
    /// </summary>
    public string Assets => Path.Combine(ProjectDirectory, "Assets");

    /// <summary>
    /// Path to the Unity project's Resources folder (Assets/Resources).
    /// </summary>
    public string Resources => Path.Combine(Assets, "Resources");

    /// <summary>
    /// Path to the Unity project's BuildData folder (Assets/BuildData).
    /// </summary>
    public string BuildData => Path.Combine(Assets, "BuildData");

    /// <summary>
    /// Returns the path to a subdirectory under Resources.
    /// </summary>
    /// <param name="subdirectories">Path segments under Resources (e.g. "Text", "Ink" => Resources/Text/Ink).</param>
    public string ResourcesDirectory(params string[] subdirectories)
    {
        if (subdirectories is null || subdirectories.Length == 0)
            return Resources;
        return Path.Combine(Resources, Path.Combine(subdirectories));
    }
}
