using System;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Backend;

public static class Utils
{
    public static void ConvertNewDiaryParameterToStatusAndType(NewDiaryEntryParameter newDiaryEntryParameter, out int status)
    {
        switch (newDiaryEntryParameter)
        {
            case NewDiaryEntryParameter.OneTime:
                status = (int)DiaryEntryStatus.OneTime;
                break;
            case NewDiaryEntryParameter.Started:
                status = (int)DiaryEntryStatus.InProgress;
                break;
            case NewDiaryEntryParameter.Finished:
                status = (int)DiaryEntryStatus.Completed;
                break;
            case NewDiaryEntryParameter.Dropped:
                status = (int)DiaryEntryStatus.Dropped;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(newDiaryEntryParameter), newDiaryEntryParameter, null);
        }
    }

    public static DateTimeOffset GetMostRelevantDate(DiaryEntryViewModel vm) => vm.Date ?? vm.EndDate ?? vm.LogDate;
    public static DateTimeOffset GetMostRelevantDate(DiaryEntry m) => m.Date ?? m.EndDate ?? m.LogDate;
    
    public enum DiaryEntryStatus
    {
        OneTime = 0,
        InProgress = 1,
        Completed = 2,
        Dropped = 3,
    }
    
    public enum EndDiaryParameter
    {
        Finished = 0,
        Dropped = 1,
    }
    
    public enum NewDiaryEntryParameter
    {
        OneTime = 0,
        Started = 1,
        Finished = 2,
        Dropped = 3,
    }
}