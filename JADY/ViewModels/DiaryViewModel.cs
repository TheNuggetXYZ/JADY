using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JADY.Backend;
using JADY.Models;
using JADY.Views;

namespace JADY.ViewModels;

public partial class DiaryViewModel : ViewModelBase
{
    /// <summary>
    /// The name of the diary.
    /// </summary>
    [ObservableProperty] private string? _name;

    /// <summary>
    /// The entries in the diary.
    /// </summary>
    public ObservableCollection<DiaryEntryViewModel> Entries { get; set; }
    
    private readonly MainWindowViewModel _mainWindowViewModel;

    public DiaryViewModel(Diary diary, MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        Name = diary.Name;
        Entries = InitializeEntries(diary.Entries);
    }

    /// <returns>
    /// a model using this view model's values.
    /// </returns>
    public Diary GetModel()
    {
        return new()
        {
            Name = Name,
            Entries = DeinitializeEntries(Entries)
        };
    }

    /// <summary>
    /// Converts a list of models to a list of their corresponding view models using the view model's constructor.
    /// </summary>
    private ObservableCollection<DiaryEntryViewModel> InitializeEntries(List<DiaryEntry> entryModels)
    {
        ObservableCollection<DiaryEntryViewModel> entryViewModels = new();

        foreach (var entryModel in entryModels)
        {
            DiaryEntryViewModel newDiaryEntryViewModel = new(entryModel, _mainWindowViewModel, this);
            entryViewModels.Add(newDiaryEntryViewModel);
        }
        
        return entryViewModels;
    }
    
    /// <summary>
    /// Converts a list of view models to a list of their corresponding models using the view model's GetModel method.
    /// </summary>
    private List<DiaryEntry> DeinitializeEntries(ObservableCollection<DiaryEntryViewModel> entryViewModels)
    {
        List<DiaryEntry> entryModels = new();

        foreach (var entryViewModel in entryViewModels)
        {
            DiaryEntry newDiaryEntryModel = entryViewModel.GetModel();
            entryModels.Add(newDiaryEntryModel);
        }
        
        return entryModels;
    }

    [RelayCommand]
    private void Remove()
    {
        _mainWindowViewModel.RemoveMyself(this);
    }

    public void RemoveMyself(DiaryEntryViewModel item)
    {
        Entries.Remove(item);
    }

    [RelayCommand]
    private async Task Edit()
    {
        Diary model = await WindowManager.OpenDialogWindow<EditDiaryWindow, Diary>(WindowManager.GetMainWindow(), this);

        if (model == null)
            return;
        
        Name = model.Name;
    }
}