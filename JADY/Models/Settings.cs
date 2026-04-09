namespace JADY.Models;

public class Settings
{
    /// <summary>
    /// Should entries marked as hidden be displayed.
    /// </summary>
    public bool ShowHiddenEntries { get; set; }

    /// <summary>
    /// The path to the directory, that JADY saves should be saved in.
    /// </summary>
    public string? SaveFilePath { get; set; }
}