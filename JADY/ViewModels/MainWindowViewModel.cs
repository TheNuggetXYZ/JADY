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

    public MainWindowViewModel()
    {
        Load();
    }
    
    [RelayCommand]
    private async Task Menu_OpenAddDiaryWindow()
    {
        // Open dialog and wait for result model
        Diary model = await WindowManager.OpenDialogWindow<AddDiaryWindow, Diary>(WindowManager.GetMainWindow(), this);

        if (model == null)
            return;
        
        // Construct and add a view model from model
        Diaries.Add(new DiaryViewModel(model, this));
    }

    [RelayCommand]
    private async Task Menu_OpenAddEntryWindow()
    {
        // Open dialog and wait for result model
        DiaryEntry model = await WindowManager.OpenDialogWindow<AddEntryWindow, DiaryEntry>(WindowManager.GetMainWindow(), this);

        if (model == null)
            return;
        
        // Construct and add a view model from model
        Diaries[OpenDiaryIndex].Entries.Add(new DiaryEntryViewModel(model, this, Diaries[OpenDiaryIndex]));
    }
    
    [RelayCommand]
    private void Menu_OpenSettingsWindow()
    {
        // Open dialog and wait for result model
        WindowManager.OpenDialogWindow<SettingsWindow, object?>(WindowManager.GetMainWindow(), this);
    }

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