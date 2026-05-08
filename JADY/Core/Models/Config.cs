using System;
using System.Globalization;
using System.Text.Json.Serialization;
using JADY.Core.Data;

namespace JADY.Core.Models;

public class Config
{
    public Config()
    {
        ShowHiddenEntries = false;
        AppTheme = AppTheme.System;
        AutoSave = false;
        CultureInfoName = "en-US";
    }
    
    /// <summary>
    /// The visual theme of the app
    /// </summary>
    public AppTheme AppTheme { get; set; }

    /// <summary>
    /// Should the app automatically save?
    /// </summary>
    public bool AutoSave { get; init; }

    /// <summary>
    /// Should entries marked as hidden be displayed?
    /// </summary>
    public bool ShowHiddenEntries
    {
        get;
        init
        {
            field = value;
            CurrentShowHiddenEntries = value;
        }
    }
    
    [JsonIgnore]
    public bool CurrentShowHiddenEntries { get; set; }

    
    /// <summary>
    /// Should the app automatically save?
    /// </summary>
    public bool AutoSave { get; init; }

    public string CultureInfoName
    {
        get;
        init
        {
            field = value;
            CultureInfo = CultureInfo.GetCultureInfo(CultureInfoName);
        }
    } = "en-US";
    
    
    [JsonIgnore]
    public CultureInfo CultureInfo { get; set; }
}