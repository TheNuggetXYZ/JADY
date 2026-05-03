using System;

namespace JADY.Core.Models;

public class DiarySubEntry
{
    /// <summary>
    /// The date at the time the sub entry was added.
    /// </summary>
    public DateTimeOffset LogDate { get; set; }
    
    /// <summary>
    /// The date of a sub entry.
    /// </summary>
    public DateTimeOffset Date { get; set; }
    
    /// <summary>
    /// E.g.:
    /// I automated red science.
    /// I finnaly destroyed those biter nests.
    /// </summary>
    public string? Content { get; set; }
}