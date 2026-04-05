using System;
using System.Collections.ObjectModel;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Backend;

public static class Utils
{
    public static Diary[] ConvertDiaryVMObservableCollectionToDiaryMArray(ObservableCollection<DiaryViewModel> diaryViewModels)
    {
        Diary[] diaries = new Diary[diaryViewModels.Count];

        for (int i = 0; i < diaryViewModels.Count; i++)
        {
            diaries[i] = diaryViewModels[i].GetModel();
        }

        return diaries;
    }

    public static ObservableCollection<DiaryViewModel> ConvertDiaryMArrayToDiaryVMObservableCollection(Diary[] diaries, MainWindowViewModel mainWindowViewModel)
    {
        ObservableCollection<DiaryViewModel> diaryViewModelObservableCollection = new();

        for (int i = 0; i < diaries.Length; i++)
        {
            diaryViewModelObservableCollection.Add(new DiaryViewModel(diaries[i], mainWindowViewModel));
        }
        
        return diaryViewModelObservableCollection;
    }
    
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
}