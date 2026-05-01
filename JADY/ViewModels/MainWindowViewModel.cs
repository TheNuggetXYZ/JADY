using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Backend;
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
    private Geometry? _saveStateIcon;
    
    [SaveDependent]
    public bool ShowHiddenEntries
    {
        get => _saveService.JadySave.Settings.CurrentShowHiddenEntries;
        set
        {
            if (_saveService.JadySave.Settings.CurrentShowHiddenEntries != value)
            {
                _saveService.JadySave.Settings.CurrentShowHiddenEntries = value;
                WeakReferenceMessenger.Default.Send(new Messages.SaveChangeMessage());
                
                OnPropertyChanged();
            }
        }
    }

    public MainWindowViewModel(ISaveService saveService, IDiaryViewModelFactory diaryViewModelFactory)
    {
        _saveService = saveService;
        _diaryViewModelFactory = diaryViewModelFactory;
        
        if (Application.Current is not null)
            SaveStateIcon = Application.Current.FindResource("RoundCheckIconSolid") as Geometry;
        
        Load();
        
        WeakReferenceMessenger.Default.Register<Messages.UnsavedChangeMessage>(this, (r, m) =>
        {
            if (Application.Current is not null)
                SaveStateIcon = Application.Current.FindResource("RoundExclamationIconSolid") as Geometry;
            
            if (_saveService.JadySave.Settings.AutoSave)
                Save();
        });
        
        WeakReferenceMessenger.Default.Register<Messages.DiariesSaveMessage>(this, (r, m) =>
        {
            if (Application.Current is not null)
                SaveStateIcon = Application.Current.FindResource("RoundCheckIconSolid") as Geometry;
        });
    }
    
    [RelayCommand]
    private async Task Menu_OpenAddDiaryWindow()
    {
        // Open dialog and wait for result model
        Diary? model = await WindowManager.OpenDialogWindowDI<AddDiaryWindow, Diary?>(WindowManager.GetMainWindow());

        if (model == null)
            return;
        
        // Construct and add a view model from model
        Diaries.Add(_diaryViewModelFactory.Create(model, this));
        
        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeMessage());
    }

    [RelayCommand]
    private async Task Menu_OpenAddEntryWindow()
    {
        if (OpenDiaryIndex < 0 || OpenDiaryIndex >= Diaries.Count)
            return;
        
        // Open dialog and wait for result model
        DiaryEntry? model = await WindowManager.OpenDialogWindowDI<AddEntryWindow, DiaryEntry?>(WindowManager.GetMainWindow());

        if (model == null)
            return;
        
        // Construct and add a view model from model
        Diaries[OpenDiaryIndex].AddEntry(model);
    }
    
    [RelayCommand]
    private async Task Menu_OpenSettingsWindow()
    {
        await WindowManager.OpenDialogWindowDI<SettingsWindow, object?>(WindowManager.GetMainWindow());
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

    public void RemoveDiary(DiaryViewModel item)
    {
        Diaries.Remove(item);
        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeMessage());
    }
}
