using JADY.Core.Data;

namespace JADY.Core.Helpers;

public static class EntryStatusExtensions
{
    public static bool IsLink(EntryStatus status) => status is EntryStatus.LinkNote or EntryStatus.LinkEndNote;

    public static bool IsEvent(EntryStatus status) =>
        status is EntryStatus.EventCompleted or EntryStatus.EventDropped or EntryStatus.EventInProgress;
    
    public static bool IsInProgress(EntryStatus status) => status is EntryStatus.EventInProgress;

    public static string[] DisplayValues = new[]
        { "One time", "Event - In progress", "Event - Completed", "Event - Dropped", "Link - Note", "Link - End note" };
}