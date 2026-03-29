using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JADY.Models;

namespace JADY.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<DiaryEntryViewModel> DiaryEntries { get; } = new();

    #region NewEntryArguments
    
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddEntryCommand))]
    private NewDiaryEntryParameter _newEntryEntryParameter;
    
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddEntryCommand))]
    private string? _newEntryCategory;
    
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddEntryCommand))] 
    private string? _newEntryTitle;
    
    [ObservableProperty] 
    private string? _newEntryContent;

    #endregion
    
    private bool CanAddEntry() => !string.IsNullOrWhiteSpace(NewEntryTitle);
    
    [RelayCommand(CanExecute = nameof(CanAddEntry))]
    private void AddEntry()
    {
        DiaryEntry newDiaryEntry = new DiaryEntry()
        {
            Category = NewEntryCategory,
            Title = NewEntryTitle,
            Content = NewEntryContent,
            LogDate = DateTime.Now,
            
            //TODO: set status and type
            //TODO: set date and end date
        };

        DiaryEntryViewModel newDiaryEntryViewModel = new DiaryEntryViewModel(newDiaryEntry);
        
        DiaryEntries.Add(newDiaryEntryViewModel);
    }
}

public enum NewDiaryEntryParameter
{
    OneTime = 0,
    Started = 1,
    Finished = 2,
    Dropped = 3,
}