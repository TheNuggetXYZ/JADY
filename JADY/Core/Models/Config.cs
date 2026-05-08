using System;
using System.Globalization;
using System.Text.Json.Serialization;
using JADY.Core.Data;

namespace JADY.Core.Models;

public class Config
{
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
    /// The visual theme of the app
    /// </summary>
    public AppTheme AppTheme
    {
        get; 
        init
        {
            field = value;
            IsThemeDark = AppTheme == AppTheme.Dark;
        }
    }
    
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
    public bool IsThemeDark {get; set;}
    
    [JsonIgnore]
    public CultureInfo CultureInfo { get; set; }
}