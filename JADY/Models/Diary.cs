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
    public DiaryEntry[]? Entries { get; set; }
}