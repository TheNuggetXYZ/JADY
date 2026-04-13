using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace JADY.Models;

public class Settings
{
    /// <summary>
    /// Should entries marked as hidden be displayed.
    /// </summary>
    public bool ShowHiddenEntries { get; init; }

    /// <summary>
    /// The path to the directory, that JADY saves should be saved in.
    /// </summary>
    public string? SaveFilePath { get; set; }

    public string CultureInfoName {get; init;} = "en-US";
    
    [JsonIgnore]
    public CultureInfo CultureInfo { get; set; }

    public Settings()
    {
        SaveFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        CultureInfo = CultureInfo.GetCultureInfo(CultureInfoName);
    }

    public void Load()
    {
        CultureInfo = CultureInfo.GetCultureInfo(CultureInfoName);
    }
}