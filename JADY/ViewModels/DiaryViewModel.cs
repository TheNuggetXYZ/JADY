using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public ObservableCollection<DiaryEntryViewModel> Entries { get; }
    
    private readonly MainWindowViewModel _mainWindowViewModel;

    public DiaryViewModel(Diary diary, MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        Name = diary.Name;
        Entries = InitializeEntriesSorted(diary.Entries);
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
    /// Converts a list of models to a sorted list of their corresponding view models using the view model's constructor.
    /// </summary>
    private ObservableCollection<DiaryEntryViewModel> InitializeEntriesSorted(List<DiaryEntry> entryModels)
    {
        IEnumerable<DiaryEntry> sortedModels = entryModels.OrderBy(Utils.GetMostRelevantDate);
        
        ObservableCollection<DiaryEntryViewModel> entryViewModels = new();

        foreach (var entryModel in sortedModels)
        {
            DiaryEntryViewModel newDiaryEntryViewModel = new(entryModel, this);
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
            entryModels.Add(entryViewModel.GetModel());
            entryViewModel.Dispose();
        }
        
        return entryModels;
    }

    public void AddEntry(DiaryEntryViewModel vm)
    {
        var key = Utils.GetMostRelevantDate(vm);
        int i = 0;
        while (i < Entries.Count && Utils.GetMostRelevantDate(Entries[i]) <= key) i++;
        Entries.Insert(i, vm);
    }

    [RelayCommand]
    private void ContextMenu_Remove()
    {
        _mainWindowViewModel.RemoveMyself(this);
    }

    [RelayCommand]
    private async Task ContextMenu_Edit()
    {
        Diary model = await WindowManager.OpenDialogWindow<EditDiaryWindow, Diary>(WindowManager.GetMainWindow(), this);

        if (model == null)
            return;
        
        Name = model.Name;
    }

    public void RemoveEntry(DiaryEntryViewModel item)
    {
        Entries.Remove(item);
    }

    public void ResortEntry(DiaryEntryViewModel item)
    {
        Entries.Remove(item);
        AddEntry(item); // readd entry
    }
}