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
        Entries = MVMConverter.ConvertModels(diary.Entries.OrderBy(Utils.GetMostRelevantDate), this);
    }

    /// <returns>
    /// a model using this view model's values.
    /// </returns>
    public Diary GetModel()
    {
        return new()
        {
            Name = Name,
            Entries = MVMConverter.ConvertViewModels(Entries)
        };
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
        _mainWindowViewModel.RemoveDiary(this);
    }

    [RelayCommand]
    private async Task ContextMenu_Edit()
    {
        Diary? model = await WindowManager.OpenDialogWindow<EditDiaryWindow, Diary?>(WindowManager.GetMainWindow(), this);

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