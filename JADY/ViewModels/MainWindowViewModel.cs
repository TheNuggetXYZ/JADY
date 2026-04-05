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
    private ObservableCollection<DiaryViewModel> _diaries = new();

    public ObservableCollection<DiaryViewModel> Diaries
    {
        get => _diaries;
        private set
        {
            if (_diaries != value)
            {
                _diaries = value;
                OnPropertyChanged();
            }
        }
    }
    
    [ObservableProperty] 
    private int _openDiaryIndex;
    
    private AddEntryWindow _addEntryWindow = new();
    private AddDiaryWindow _addDiaryWindow = new();

    #region NewDiaryArguments

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddDiaryWindow_AddCommand))] 
    private string? _newDiaryName;

    #endregion

    #region NewEntryArguments
    
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddEntryWindow_AddCommand))]
    private NewDiaryEntryParameter _newEntryParameter;

    public Array NewEntryParameterValues => Enum.GetValues(typeof(NewDiaryEntryParameter));

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddEntryWindow_AddCommand))]
    private string? _newEntryCategory;
    
    [ObservableProperty]
    private string? _newEntrySubCategory;
    
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(AddEntryWindow_AddCommand))] 
    private string? _newEntryTitle;
    
    [ObservableProperty] 
    private string? _newEntryContent;

    [ObservableProperty] 
    private bool _newEntryIsHidden;
    
    [ObservableProperty] 
    private DateTimeOffset? _newEntryDate;

    #endregion

    public MainWindowViewModel()
    {
        Load();
    }

    private bool Can_AddDiaryWindow_Add() => !string.IsNullOrWhiteSpace(NewDiaryName);
    private bool Can_AddEntryWindow_Add() => !string.IsNullOrWhiteSpace(NewEntryTitle);
    private bool Can_ContextMenu_OpenAddEntryWindow() => !_addEntryWindow.IsVisible;
    private bool Can_ContextMenu_OpenAddDiaryWindow() => !_addDiaryWindow.IsVisible;
    
    [RelayCommand(CanExecute = nameof(Can_AddDiaryWindow_Add))]
    private void AddDiaryWindow_Add()
    {
        Diary newDiary = new Diary()
        {
            Name = NewDiaryName,
            Entries = new()
        };
        
        DiaryViewModel newDiaryViewModel = new DiaryViewModel(newDiary, this);
        
        Diaries.Add(newDiaryViewModel);
        
        _addDiaryWindow.Close();
        ResetNewDiaryArguments();
    }
    
    [RelayCommand(CanExecute = nameof(Can_AddEntryWindow_Add))]
    private void AddEntryWindow_Add()
    {
        if (OpenDiaryIndex >= Diaries.Count || OpenDiaryIndex < 0)
            return;
        
        ConvertNewDiaryParameterToStatusAndType(NewEntryParameter, out int status, out int type);
        
        DiaryEntry newDiaryEntry = new DiaryEntry()
        {
            Category = NewEntryCategory,
            SubCategory = NewEntrySubCategory,
            LogDate = DateTimeOffset.Now,
            Date = NewEntryDate,
            Title = NewEntryTitle,
            Content = NewEntryContent,
            IsHidden = NewEntryIsHidden,
            Status = (DiaryEntryStatus)status,
            Type = (DiaryEntryType)type
        };

        DiaryEntryViewModel newDiaryEntryViewModel = new DiaryEntryViewModel(newDiaryEntry, this, Diaries[OpenDiaryIndex]);
        
        Diaries[OpenDiaryIndex].Entries.Add(newDiaryEntryViewModel);
        
        _addEntryWindow.Close();
        ResetNewEntryArguments();
    }

    [RelayCommand(CanExecute = nameof(Can_ContextMenu_OpenAddDiaryWindow))]
    private async void ContextMenu_OpenAddDiaryWindow()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _addDiaryWindow = new AddDiaryWindow { DataContext = this };
            await _addDiaryWindow.ShowDialog(desktop.MainWindow);
            ContextMenu_OpenAddDiaryWindowCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand(CanExecute = nameof(Can_ContextMenu_OpenAddEntryWindow))]
    private async void ContextMenu_OpenAddEntryWindow()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _addEntryWindow = new AddEntryWindow { DataContext = this };
            await _addEntryWindow.ShowDialog(desktop.MainWindow);
            ContextMenu_OpenAddEntryWindowCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private void ContextMenu_Save() => Save();

    [RelayCommand]
    private void ContextMenu_Load() => Load();

    private void Save()
    {
        DiaryJSON.Save(ConvertDiaryViewModelObservableCollectionToDiaryModelArray(Diaries));
    }

    private void Load()
    {
        DiaryJSON.Load();

        Diaries = ConvertDiaryArrayToDiaryViewModelObservableCollection(DiaryJSON.JadySave.Diaries);
    }

    private void ResetNewDiaryArguments()
    {
        NewDiaryName = string.Empty;
    }

    private void ResetNewEntryArguments()
    {
        NewEntryParameter = NewDiaryEntryParameter.OneTime;
        NewEntryCategory = string.Empty;
        NewEntrySubCategory = string.Empty;
        NewEntryTitle = string.Empty;
        NewEntryContent = string.Empty;
        NewEntryDate = null;
        NewEntryIsHidden = false;
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
            diaryViewModelObservableCollection.Add(new DiaryViewModel(diaries[i], this));
        }
        
        return diaryViewModelObservableCollection;
    }

    public void RemoveMyself(DiaryViewModel item)
    {
        Diaries.Remove(item);
    }
}

public enum NewDiaryEntryParameter
{
    OneTime = 0,
    Started = 1,
    Finished = 2,
    Dropped = 3,
}