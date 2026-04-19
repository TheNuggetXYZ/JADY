using System;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Backend;

public static class Utils
{
    public static void ConvertNewDiaryParameterToStatusAndType(NewEntryParameter newEntryParameter, out int status)
    {
        switch (newEntryParameter)
        {
            case NewEntryParameter.OneTime:
                status = (int)EntryStatus.OneTime;
                break;
            case NewEntryParameter.Started:
                status = (int)EntryStatus.InProgress;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(newEntryParameter), newEntryParameter, null);
        }
    }

    public static DateTimeOffset GetMostRelevantDate(DiaryEntryViewModel vm) => vm.Date ?? vm.EndDate ?? vm.LogDate;
    public static DateTimeOffset GetMostRelevantDate(DiaryEntry m) => m.Date ?? m.EndDate ?? m.LogDate;
    
    public enum EntryStatus
    {
        OneTime = 0,
        InProgress = 1,
        Completed = 2,
        Dropped = 3,
    }
    
    public enum NewEntryParameter
    {
        OneTime = 0,
        Started = 1,
    }
    
    public enum EndEntryParameter
    {
        Completed = 0,
        Dropped = 1,
    }
}