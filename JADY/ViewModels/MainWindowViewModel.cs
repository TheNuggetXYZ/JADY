using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JADY.Backend;
using JADY.Models;
using JADY.Views;
using DiaryEntry = JADY.Models.DiaryEntry;

namespace JADY.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<DiaryViewModel> Diaries { get; } = new();
    
    [ObservableProperty] 
    private int _openDiaryIndex;
    
    private AddEntryWindow _addEntryWindow = new();

    #region NewDiaryArguments

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddDiaryCommand))] 
    private string? _newDiaryName;

    #endregion

    #region NewEntryArguments
    
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddEntryCommand))]
    private NewDiaryEntryParameter _newEntryParameter;
    
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddEntryCommand))]
    private string? _newEntryCategory;
    
    [ObservableProperty]
    private string? _newEntrySubCategory;
    
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddEntryCommand))] 
    private string? _newEntryTitle;
    
    [ObservableProperty] 
    private string? _newEntryContent;
    
    [ObservableProperty] 
    private DateTime? _newEntryDate;
    
    [ObservableProperty] 
    private DateTime? _newEntryEndDate;

    #endregion

    private bool CanAddDiary() => !string.IsNullOrWhiteSpace(NewDiaryName);
    
    [RelayCommand(CanExecute = nameof(CanAddDiary))]
    private void AddDiary()
    {
        Diary newDiary = new Diary()
        {
            Name = NewDiaryName,
            Entries = new()
        };
        
        DiaryViewModel newDiaryViewModel = new DiaryViewModel(newDiary);
        
        Diaries.Add(newDiaryViewModel);
    }
    
    private bool CanAddEntry() => !string.IsNullOrWhiteSpace(NewEntryTitle);
    
    [RelayCommand(CanExecute = nameof(CanAddEntry))]
    private void AddEntry()
    {
        if (OpenDiaryIndex >= Diaries.Count || OpenDiaryIndex < 0)
            return;
        
        ConvertNewDiaryParameterToStatusAndType(NewEntryParameter, out int status, out int type);
        
        DiaryEntry newDiaryEntry = new DiaryEntry()
        {
            Category = NewEntryCategory,
            Title = NewEntryTitle,
            Content = NewEntryContent,
            LogDate = DateTime.Now,
            Status = (DiaryEntryStatus)status,
            Type = (DiaryEntryType)type

            //TODO: set date and end date
        };

        DiaryEntryViewModel newDiaryEntryViewModel = new DiaryEntryViewModel(newDiaryEntry);
        
        Diaries[OpenDiaryIndex].Entries.Add(newDiaryEntryViewModel);
        
        DiaryJSON.Save(ConvertDiaryViewModelObservableCollectionToDiaryModelArray(Diaries));
    }

    private bool CanOpenAddEntryWindow() => !_addEntryWindow.IsVisible;

    [RelayCommand(CanExecute = nameof(CanOpenAddEntryWindow))]
    private async void OpenAddEntryWindow()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _addEntryWindow = new AddEntryWindow { DataContext = this };
            await _addEntryWindow.ShowDialog(desktop.MainWindow);
            OpenAddEntryWindowCommand.NotifyCanExecuteChanged();
        }
    }

    private void ConvertNewDiaryParameterToStatusAndType(NewDiaryEntryParameter newDiaryEntryParameter, out int status, out int type)
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

    private Diary[] ConvertDiaryViewModelObservableCollectionToDiaryModelArray(ObservableCollection<DiaryViewModel> diaryViewModels)
    {
        Diary[] diaries = new Diary[diaryViewModels.Count];

        for (int i = 0; i < diaryViewModels.Count; i++)
        {
            diaries[i] = diaryViewModels[i].GetModel();
        }

        return diaries;
    }

    private ObservableCollection<DiaryViewModel> ConvertDiaryArrayToDiaryViewModelObservableCollection(Diary[] diaries)
    {
        ObservableCollection<DiaryViewModel> diaryViewModelObservableCollection = new();

        for (int i = 0; i < diaries.Length; i++)
        {
            diaryViewModelObservableCollection.Add(new DiaryViewModel(diaries[i]));
        }
        
        return diaryViewModelObservableCollection;
    }
}

public enum NewDiaryEntryParameter
{
    OneTime = 0,
    Started = 1,
    Finished = 2,
    Dropped = 3,
}