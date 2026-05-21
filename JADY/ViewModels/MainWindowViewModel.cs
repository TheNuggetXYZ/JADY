using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Core;
using JADY.Core.Attributes;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.Services;
using JADY.UI.Views.Dialogs;

namespace JADY.ViewModels;

public partial class MainWindowViewModel(IDiaryService diaryService, ISaveService saveService, IWindowService windowService) : SaveDependentViewModel
{
    public ISaveService SaveService { get; } = saveService;

    public ObservableCollection<DiaryViewModel> Diaries => diaryService.Diaries;

    [ObservableProperty] 
    private int _openDiaryIndex;
    
    [SaveDependent]
    public bool ShowHiddenEntries
    {
        get => SaveService.Config.CurrentShowHiddenEntries;
        set
        {
            if (SaveService.Config.CurrentShowHiddenEntries != value)
            {
                SaveService.Config.CurrentShowHiddenEntries = value;
                WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
                
                OnPropertyChanged();
            }
        }
    }

    [ObservableProperty]
    private bool _isEntrySearchBarVisible;
    
    [ObservableProperty]
    private string? _searchBoxText;

    public async Task<bool> OnClosing()
    {
        bool cancelClosing = false;
        
        if (SaveService.UnsavedChanges)
        {
            cancelClosing = true;
            
            Optional<UnsavedChangesChoice> choice = await windowService.OpenDialogWindowDI<UnsavedChangesWindow, UnsavedChangesChoice>(windowService.GetMainWindow());

            if (!choice.HasValue) // closed window
            {
                return cancelClosing;
            }
            
            if (choice.Value == UnsavedChangesChoice.Quit)
            {
                cancelClosing = false;
            }
            else if (choice.Value == UnsavedChangesChoice.Save)
            {
                diaryService.SaveDiaries();
                
                cancelClosing = false;
            }
        }

        return cancelClosing;
    }

    partial void OnIsEntrySearchBarVisibleChanged(bool value)
    {
        if (!value)
        {
            Diaries[OpenDiaryIndex].RemoveFilter();
        }
    }

    [RelayCommand]
    private void SearchBoxSubmit()
    {
        Diaries[OpenDiaryIndex].SetFilter(SearchBoxText);
    }

    [RelayCommand]
    private async Task Menu_OpenAddDiaryWindow()
    {
        // Open dialog and wait for result model
        Optional<Diary> model = await windowService.OpenDialogWindowDI<AddDiaryWindow, Diary>(windowService.GetMainWindow());

        if (model.HasValue)
            diaryService.AddDiary(model.Value);
    }

    [RelayCommand]
    private async Task Menu_OpenAddEntryWindow()
    {
        if (OpenDiaryIndex < 0 || OpenDiaryIndex >= Diaries.Count)
            return;
        
        // Open dialog and wait for result model
        Optional<DiaryEntry> model = await windowService.OpenDialogWindowDI<AddEntryWindow, DiaryEntry>(windowService.GetMainWindow());

        if (model.HasValue)
            diaryService.AddEntry(model.Value, OpenDiaryIndex);
    }
    
    [RelayCommand]
    private async Task Menu_OpenSettingsWindow()
    {
        await windowService.OpenDialogWindowDI<SettingsWindow, Config>(windowService.GetMainWindow());
    }
    
    [RelayCommand]
    private void Menu_ToggleHiddenEntries()
    {
        ShowHiddenEntries = !ShowHiddenEntries;
    }

    [RelayCommand]
    private void Menu_Save() => diaryService.SaveDiaries();

    [RelayCommand]
    private void Menu_Load() => diaryService.LoadDiaries(true);
}
