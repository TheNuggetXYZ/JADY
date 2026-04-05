using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

    private bool Can_AddEntryWindow_Add() => !string.IsNullOrWhiteSpace(NewEntryTitle);
    
    [RelayCommand(CanExecute = nameof(Can_AddEntryWindow_Add))]
    private void AddEntryWindow_Add()
    {
        if (OpenDiaryIndex >= Diaries.Count || OpenDiaryIndex < 0)
            return;
        
        Utils.ConvertNewDiaryParameterToStatusAndType(NewEntryParameter, out int status, out int type);
        
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
        
        WindowManager.CloseWindow<AddEntryWindow>();
        ResetNewEntryArguments();
    }

    [RelayCommand]
    private async Task Menu_OpenAddDiaryWindow()
    {
        // Open dialog and wait for result model
        Diary diary = await WindowManager.OpenDialogWindow<AddDiaryWindow, Diary>(WindowManager.GetMainWindow(), this);
        
        // Construct and add a view model from model
        Diaries.Add(new DiaryViewModel(diary, this));
    }

    [RelayCommand]
    private void Menu_OpenAddEntryWindow() =>
        WindowManager.OpenDialogWindow<AddEntryWindow, DiaryEntry>(WindowManager.GetMainWindow(), this);

    [RelayCommand]
    private void Menu_Save() => Save();

    [RelayCommand]
    private void Menu_Load() => Load();

    private void Save()
    {
        DiaryJSON.Save(Utils.ConvertDiaryVMObservableCollectionToDiaryMArray(Diaries));
    }

    private void Load()
    {
        DiaryJSON.Load();

        Diaries = Utils.ConvertDiaryMArrayToDiaryVMObservableCollection(DiaryJSON.JadySave.Diaries, this);
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