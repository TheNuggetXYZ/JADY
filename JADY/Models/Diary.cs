using System.Collections.Generic;

namespace JADY.Models;

public class Diary
{
    /// <summary>
    /// The name of the diary.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The entries in the diary.
    /// </summary>
    public List<DiaryEntry> Entries { get; set; } = new();
}