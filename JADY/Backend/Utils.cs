using System;
using System.IO;
using JADY.Data;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Backend;

public static class Utils
{
    public static EntryStatus NewEntryParameterToEntryStatus(NewEntryParameter newEntryParameter)
    {
        return newEntryParameter switch
        {
            NewEntryParameter.OneTime => EntryStatus.OneTime,
            NewEntryParameter.Started => EntryStatus.InProgress,
            _ => throw new ArgumentOutOfRangeException(nameof(newEntryParameter), newEntryParameter, null)
        };
    }

    public static DateTimeOffset GetMostRelevantDate(DiaryEntryViewModel vm) => vm.Date ?? vm.EndDate ?? vm.LogDate;
    public static DateTimeOffset GetMostRelevantDate(DiaryEntry m) => m.Date ?? m.EndDate ?? m.LogDate;
    
    public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
    {
        try
        {
            using (FileStream fs = File.Create(
                       Path.Combine(
                           dirPath, 
                           Path.GetRandomFileName()
                       ), 
                       1,
                       FileOptions.DeleteOnClose)
                  )
            { }
            return true;
        }
        catch
        {
            if (throwIfFails)
                throw;
            else
                return false;
        }
    }

    public static readonly string[] EntryStatusToArray = ["One time", "In progress", "Completed", "Dropped"];
    public static readonly string[] NewEntryParameterToArray = ["One time", "Started"];
    public static readonly string[] EndEntryParameterToArray = ["Completed", "Dropped"];
}