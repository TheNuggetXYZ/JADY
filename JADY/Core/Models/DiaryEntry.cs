using System;
using System.Collections.Generic;
using JADY.Core.Data;

namespace JADY.Core.Models;

public class DiaryEntry
{
    public EntryStatus Status { get; set; }
    
    /// <summary>
    /// The date at the time the entry was added.
    /// </summary>
    public DateTimeOffset LogDate { get; set; }
    
    /// <summary>
    /// The start date of an event or a date of a one time entry.
    /// </summary>
    public DateTimeOffset? Date { get; set; }

    /// <summary>
    /// The end date of an event. Is useless for a one time entry.
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }
    
    /// <summary>
    /// E.g. Game/Anime/Misc
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// E.g. Factorio/OPM/null
    /// </summary>
    public string? SubCategory { get; set; }
    
    /// <summary>
    /// E.g. First time playing Factorio
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// E.g. I started playing Factorio for the first time on the ... version and its ...
    /// </summary>
    public string? Content { get; set; }
    
    /// <summary>
    /// Used if you don't want this entry to appear unless you toggle on a setting to show hidden entries.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// E.g. [I automated red science, I finally destroyed those biter nests, ...]
    /// </summary>
    public List<DiarySubEntry> SubEntries { get; set; } = new();
}
