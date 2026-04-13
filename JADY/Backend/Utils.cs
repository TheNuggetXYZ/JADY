using System;
using System.Collections.ObjectModel;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Backend;

public static class Utils
{
    public static void ConvertNewDiaryParameterToStatusAndType(NewDiaryEntryParameter newDiaryEntryParameter, out int status, out int type)
    {
        switch (newDiaryEntryParameter)
        {
            case NewDiaryEntryParameter.OneTime:
                status = (int)DiaryEntryStatus.None;
                type = (int)DiaryEntryType.OneTime;
                break;
            case NewDiaryEntryParameter.Started:
                status = (int)DiaryEntryStatus.InProgress;
                type = (int)DiaryEntryType.ProlongedEvent;
                break;
            case NewDiaryEntryParameter.Finished:
                status = (int)DiaryEntryStatus.Completed;
                type = (int)DiaryEntryType.ProlongedEvent;
                break;
            case NewDiaryEntryParameter.Dropped:
                status = (int)DiaryEntryStatus.Dropped;
                type = (int)DiaryEntryType.ProlongedEvent;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(newDiaryEntryParameter), newDiaryEntryParameter, null);
        }
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