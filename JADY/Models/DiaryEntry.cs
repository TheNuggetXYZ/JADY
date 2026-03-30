using System;

namespace JADY.Models;

public class DiaryEntry
{
    public DiaryEntryType Type { get; set; }
    
    public DiaryEntryStatus Status { get; set; }
    
    /// <summary>
    /// The date at the time the entry was added.
    /// </summary>
    public DateTime LogDate { get; set; }
    
    /// <summary>
    /// The start date of an event or a date of a one time entry.
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// The end date of an event. Is useless for a one time entry.
    /// </summary>
    public DateTime? EndDate { get; set; }
    
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
    /// E.g. [I automated red science, I finally destroyed those biter nests, ...]
    /// </summary>
    public DiarySubEntry[]? SubEntries { get; set; }
}

public enum DiaryEntryType
{
    OneTime = 0,
    ProlongedEvent = 1
}

public enum DiaryEntryStatus
{
    None = 0,
    InProgress = 1,
    Completed = 2,
    Dropped = 3,
}