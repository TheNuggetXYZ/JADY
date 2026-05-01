using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace JADY.Models;

public class Settings
{
    /// <summary>
    /// Should entries marked as hidden be displayed?
    /// </summary>
    private readonly bool _showHiddenEntries;

    public bool ShowHiddenEntries
    {
        get => _showHiddenEntries;
        init
        {
            _showHiddenEntries = value;
            CurrentShowHiddenEntries = value;
        }
    }
    
    [JsonIgnore]
    public bool CurrentShowHiddenEntries { get; set; }
    
    /// <summary>
    /// Should the apps theme be set to dark?
    /// </summary>
    public bool IsThemeDark { get; init; }
    
    /// <summary>
    /// Should the app automatically save?
    /// </summary>
    public bool AutoSave { get; init; }

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
        CurrentShowHiddenEntries = ShowHiddenEntries;
    }

    public void Load()
    {
        CultureInfo = CultureInfo.GetCultureInfo(CultureInfoName);
        CurrentShowHiddenEntries = ShowHiddenEntries;
    }
}