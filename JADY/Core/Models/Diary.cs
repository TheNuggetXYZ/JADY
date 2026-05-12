using System.Collections.Generic;

namespace JADY.Core.Models;

public class Diary
{
    /// <summary>
    /// The name of the diary.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// The entries in the diary.
    /// </summary>
    public List<DiaryEntry> Entries { get; init; } = new();
}