using System;

namespace JADY.Models;

public class DiaryEntry
{
    public DiaryEntryType Type { get; set; }
    
    public DiaryEntryStatus Status { get; set; }
    
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
    
    public string? Title { get; set; }
    
    public string? Content { get; set; }
    
    public DiarySubEntry[]? SubEntries { get; set; }
}

public enum DiaryEntryType
{
    Normal = 0,
    Game = 1
}

public enum DiaryEntryStatus
{
    None = 0,
    InProgress = 1,
    Completed = 2,
    Dropped = 3,
}