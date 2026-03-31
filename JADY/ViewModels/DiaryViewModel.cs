using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using JADY.Models;

namespace JADY.ViewModels;

public partial class DiaryViewModel : ViewModelBase
{
    /// <summary>
    /// The name of the diary.
    /// </summary>
    [ObservableProperty] private string? _name;

    /// <summary>
    /// The entries in the diary.
    /// </summary>
    [ObservableProperty] private List<DiaryEntryViewModel> _entries = new();

    public DiaryViewModel(Diary diary)
    {
        Name = diary.Name;
        Entries = InitializeEntries(diary.Entries);
    }

    private List<DiaryEntryViewModel> InitializeEntries(List<DiaryEntry> entryModels)
    {
        List<DiaryEntryViewModel> entryViewModels = new();

        foreach (var entryModel in entryModels)
        {
            DiaryEntryViewModel newDiaryEntryViewModel = new(entryModel);
            entryViewModels.Add(newDiaryEntryViewModel);
        }
        
        return entryViewModels;
    }
}