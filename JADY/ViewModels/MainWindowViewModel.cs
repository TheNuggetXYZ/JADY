using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Backend;
using JADY.Data;
using JADY.Factories;
using JADY.Models;
using JADY.Services;
using JADY.Views;

namespace JADY.ViewModels;

public partial class MainWindowViewModel : SaveDependentViewModel
{
    private readonly ISaveService _saveService;
    private readonly IDiaryViewModelFactory _diaryViewModelFactory;
    
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

    [ObservableProperty] 
    private object? _saveState;
    
    [SaveDependent]
    public bool ShowHiddenEntries
    {
        get => _saveService.JadySave.Settings.CurrentShowHiddenEntries;
        set
        {
            if (_saveService.JadySave.Settings.CurrentShowHiddenEntries != value)
            {
                _saveService.JadySave.Settings.CurrentShowHiddenEntries = value;
                WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
                
                OnPropertyChanged();
            }
        }
    }

    public MainWindowViewModel(ISaveService saveService, IDiaryViewModelFactory diaryViewModelFactory)
    {
        _saveService = saveService;
        _diaryViewModelFactory = diaryViewModelFactory;
        
        SaveState = new SaveState_Saved();
        
        Load();
        
        WeakReferenceMessenger.Default.Register<Messages.UnsavedChangeCreated>(this, (r, m) =>
        {
            SaveState = new SaveState_UnsavedChanges();
        });
        
        WeakReferenceMessenger.Default.Register<Messages.DiariesSavePerformed>(this, (r, m) =>
        {
            SaveState = new SaveState_Saved();
        });

        WeakReferenceMessenger.Default.Register<Messages.PerformSave>(this, (r, m) =>
        {
            Save();
        });
    }

    public async Task<bool> OnClosing()
    {
        bool cancelClosing = false;
        
        if (_saveService.UnsavedChanges)
        {
            cancelClosing = true;
            
            Optional<UnsavedChangesChoice> choice = await WindowManager.OpenDialogWindowDI<UnsavedChangesWindow, UnsavedChangesChoice>(WindowManager.GetMainWindow());

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
        Optional<Diary> model = await WindowManager.OpenDialogWindowDI<AddDiaryWindow, Diary>(WindowManager.GetMainWindow());

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
        Optional<DiaryEntry> model = await WindowManager.OpenDialogWindowDI<AddEntryWindow, DiaryEntry>(WindowManager.GetMainWindow());

        if (!model.HasValue)
            return;
        
        // Construct and add a view model from model
        Diaries[OpenDiaryIndex].AddEntry(model.Value);
    }
    
    [RelayCommand]
    private async Task Menu_OpenSettingsWindow()
    {
        await WindowManager.OpenDialogWindowDI<SettingsWindow, Settings>(WindowManager.GetMainWindow());
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
        _saveService.Save(Diaries.Select(d => d.GetModel()).ToArray());
    }

    private void Load()
    {
        _saveService.Load();

        Diaries = new ObservableCollection<DiaryViewModel>(
            _saveService.JadySave.Diaries.Select(model => _diaryViewModelFactory.Create(model, this)));
    }

    public async Task RemoveDiary(DiaryViewModel item)
    {
        Optional<bool> pickedYes = await WindowManager.OpenYesNoMessageBox(WindowManager.GetMainWindow(), "Are you sure you want to remove this diary?", "Remove diary?");
        if (!pickedYes.HasValue || pickedYes.Value == false) return;
            
        Diaries.Remove(item);
        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeCreated());
    }
}
