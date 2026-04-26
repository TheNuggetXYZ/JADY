using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Backend;
using JADY.Factories;
using JADY.Models;
using JADY.Services;
using JADY.Views;

namespace JADY.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ISaveService _saveService;
    private readonly IDiaryEntryViewModelFactory _diaryEntryViewModelFactory;
    
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
    private float _saveIconOpacity = 0f;

    public MainWindowViewModel(ISaveService saveService, IDiaryEntryViewModelFactory diaryEntryViewModelFactory)
    {
        _saveService = saveService;
        _diaryEntryViewModelFactory = diaryEntryViewModelFactory;
        
        Load();
        
        WeakReferenceMessenger.Default.Register<Messages.PerformAutoSaveMessage>(this, (r, m) =>
        {
            if (_saveService.JadySave.Settings.AutoSave)
                Save();
        });
        
        WeakReferenceMessenger.Default.Register<Messages.AnySaveMessage>(this, async (r, m) =>
        {
            try
            {
                await SaveIconAnimation();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
        Diaries.Add(new DiaryViewModel(model, this));
        
        WeakReferenceMessenger.Default.Send(new Messages.PerformAutoSaveMessage());
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
        Diaries[OpenDiaryIndex].AddEntry(_diaryEntryViewModelFactory.Create(model, Diaries[OpenDiaryIndex])); // TODO: no need to assign owner when we call this from the owner, the owner can assign that themselves
    }
    
    [RelayCommand]
    private async Task Menu_OpenSettingsWindow()
    {
        await WindowManager.OpenDialogWindowDI<SettingsWindow, object?>(WindowManager.GetMainWindow());
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
            _saveService.JadySave.Diaries.Select(d => new DiaryViewModel(d, this)));
    }

    private async Task SaveIconAnimation()
    {
        SaveIconOpacity = 1f;
        await Task.Delay(500);
        SaveIconOpacity = 0f;
    }

    public void RemoveDiary(DiaryViewModel item)
    {
        Diaries.Remove(item);
        WeakReferenceMessenger.Default.Send(new Messages.PerformAutoSaveMessage());
    }
}
