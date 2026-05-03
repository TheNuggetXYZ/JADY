using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Core;
using JADY.Core.Attributes;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.Factories;
using JADY.Services;
using JADY.Views;
using AddDiaryWindow = JADY.Views.Dialogs.AddDiaryWindow;
using AddEntryWindow = JADY.Views.Dialogs.AddEntryWindow;
using SettingsWindow = JADY.Views.Dialogs.SettingsWindow;
using UnsavedChangesWindow = JADY.Views.Dialogs.UnsavedChangesWindow;

namespace JADY.ViewModels;

public partial class MainWindowViewModel : SaveDependentViewModel
{
    public ISaveService SaveService { get; }
    private readonly IDiaryViewModelFactory _diaryViewModelFactory;
    private readonly IWindowService _windowService;
    
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
    
    [SaveDependent]
    public bool ShowHiddenEntries
    {
        get => SaveService.JadySave.Settings.CurrentShowHiddenEntries;
        set
        {
            if (SaveService.JadySave.Settings.CurrentShowHiddenEntries != value)
            {
                SaveService.JadySave.Settings.CurrentShowHiddenEntries = value;
                WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
                
                OnPropertyChanged();
            }
        }
    }

    public MainWindowViewModel(ISaveService saveService, IDiaryViewModelFactory diaryViewModelFactory, IWindowService windowService)
    {
        SaveService = saveService;
        _diaryViewModelFactory = diaryViewModelFactory;
        _windowService = windowService;

        Load();

        WeakReferenceMessenger.Default.Register<Messages.PerformSave>(this, (r, m) =>
        {
            Save();
        });
    }

    public async Task<bool> OnClosing()
    {
        bool cancelClosing = false;
        
        if (SaveService.UnsavedChanges)
        {
            cancelClosing = true;
            
            Optional<UnsavedChangesChoice> choice = await _windowService.OpenDialogWindowDI<UnsavedChangesWindow, UnsavedChangesChoice>(_windowService.GetMainWindow());

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
                Save();
                
                cancelClosing = false;
            }
        }

        return cancelClosing;
    }

    [RelayCommand]
    private async Task Menu_OpenAddDiaryWindow()
    {
        // Open dialog and wait for result model
        Optional<Diary> model = await _windowService.OpenDialogWindowDI<AddDiaryWindow, Diary>(_windowService.GetMainWindow());

        if (!model.HasValue)
            return;
        
        // Construct and add a view model from model
        Diaries.Add(_diaryViewModelFactory.Create(model.Value, this));
        
        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeCreated());
    }

    [RelayCommand]
    private async Task Menu_OpenAddEntryWindow()
    {
        if (OpenDiaryIndex < 0 || OpenDiaryIndex >= Diaries.Count)
            return;
        
        // Open dialog and wait for result model
        Optional<DiaryEntry> model = await _windowService.OpenDialogWindowDI<AddEntryWindow, DiaryEntry>(_windowService.GetMainWindow());

        if (!model.HasValue)
            return;
        
        // Construct and add a view model from model
        Diaries[OpenDiaryIndex].AddEntry(model.Value);
    }
    
    [RelayCommand]
    private async Task Menu_OpenSettingsWindow()
    {
        await _windowService.OpenDialogWindowDI<SettingsWindow, Settings>(_windowService.GetMainWindow());
    }
    
    [RelayCommand]
    private void Menu_ToggleHiddenEntries()
    {
        ShowHiddenEntries = !ShowHiddenEntries;
    }

    [RelayCommand]
    private void Menu_Save() => Save();

    [RelayCommand]
    private void Menu_Load() => Load();

    private void Save()
    {
        SaveService.Save(Diaries.Select(d => d.GetModel()).ToArray());
    }

    private void Load()
    {
        SaveService.Load();

        Diaries = new ObservableCollection<DiaryViewModel>(
            SaveService.JadySave.Diaries.Select(model => _diaryViewModelFactory.Create(model, this)));
    }

    public async Task RemoveDiary(DiaryViewModel item)
    {
        Optional<bool> pickedYes = await _windowService.OpenYesNoMessageBox(_windowService.GetMainWindow(), "Are you sure you want to remove this diary?", "Remove diary?");
        if (!pickedYes.HasValue || pickedYes.Value == false) return;
            
        Diaries.Remove(item);
        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeCreated());
    }
}
