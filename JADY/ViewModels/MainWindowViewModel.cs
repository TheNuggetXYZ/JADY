using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JADY.Backend;
using JADY.Models;
using JADY.Views;

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
        if (OpenDiaryIndex < 0 || OpenDiaryIndex >= Diaries.Count)
            return;
        
        // Open dialog and wait for result model
        DiaryEntry model = await WindowManager.OpenDialogWindow<AddEntryWindow, DiaryEntry>(WindowManager.GetMainWindow(), this);

        if (model == null)
            return;
        
        // Construct and add a view model from model
        Diaries[OpenDiaryIndex].AddEntry(new DiaryEntryViewModel(model, Diaries[OpenDiaryIndex]));
    }
    
    [RelayCommand]
    private async Task Menu_OpenSettingsWindow()
    {
        await WindowManager.OpenDialogWindow<SettingsWindow, object?>(WindowManager.GetMainWindow(), this);
    }

    [RelayCommand]
    private void Menu_Save() => Save();

    [RelayCommand]
    private void Menu_Load() => Load();

    private void Save()
    {
        Saves.Save(Diaries.Select(d => d.GetModel()).ToArray());
    }

    private void Load()
    {
        Saves.Load();

        Diaries = new ObservableCollection<DiaryViewModel>(
            Saves.JadySave.Diaries.Select(d => new DiaryViewModel(d, this)));
    }

    public void RemoveMyself(DiaryViewModel item)
    {
        Diaries.Remove(item);
    }
}
