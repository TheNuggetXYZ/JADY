using System;
using System.Collections.Generic;
using JADY.Core.Data;

namespace JADY.Core.Models;

public class DiaryEntry
{
    public EntryStatus Status { get; init; }
    
    /// <summary>
    /// The date at the time the entry was added.
    /// </summary>
    public DateTimeOffset LogDate { get; init; }
    
    /// <summary>
    /// The start date of an event or a date of a one time entry.
    /// </summary>
    public DateTimeOffset? Date { get; init; }

    /// <summary>
    /// The end date of an event. Is useless for a one time entry.
    /// </summary>
    public DateTimeOffset? EndDate { get; init; }
    
    /// <summary>
    /// E.g. Game/Anime/Misc
    /// </summary>
    public string? Category { get; init; }
    
    /// <summary>
    /// E.g. Factorio/OPM/null
    /// </summary>
    public string? SubCategory { get; init; }
    
    /// <summary>
    /// E.g. First time playing Factorio
    /// </summary>
    public string? Title { get; init; }
    
    /// <summary>
    /// E.g. I started playing Factorio for the first time on the ... version and its ...
    /// </summary>
    public string? Content { get; init; }
    
    /// <summary>
    /// Used if you don't want this entry to appear unless you toggle on a setting to show hidden entries.
    /// </summary>
    public bool IsHidden { get; init; }
    
    /// <summary>
    /// The global unique identifier of this exact entry.
    /// </summary>
    public Guid EntryGuid { get; init; } = Guid.NewGuid();
    
    /// <summary>
    /// The global unique identifier of the parent of this entry note.
    /// </summary>
    public Guid? ParentEntryGuid { get; init; }
}
