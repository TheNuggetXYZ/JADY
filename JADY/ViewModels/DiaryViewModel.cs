using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Backend;
using JADY.Factories;
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

    private readonly IDiaryEntryViewModelFactory _diaryEntryViewModelFactory;

    public DiaryViewModel(Diary diary, MainWindowViewModel mainWindowViewModel, IDiaryEntryViewModelFactory diaryEntryViewModelFactory)
    {
        _diaryEntryViewModelFactory = diaryEntryViewModelFactory;
        _mainWindowViewModel = mainWindowViewModel;
        Name = diary.Name;
        Entries = new ObservableCollection<DiaryEntryViewModel>(diary.Entries.OrderByDescending(Utils.GetMostRelevantDate).Select(x => diaryEntryViewModelFactory.Create(x, this)));
    }

    /// <returns>
    /// a model using this view model's values.
    /// </returns>
    public Diary GetModel()
    {
        return new()
        {
            Name = Name,
            Entries = new(Entries.Select(x => x.GetModel()))
        };
    }

    public void AddEntry(DiaryEntry model)
    {
        DiaryEntryViewModel newEntryViewModel = _diaryEntryViewModelFactory.Create(model, this);
        
        InsertEntry(newEntryViewModel);
    }

    private void InsertEntry(DiaryEntryViewModel newEntryViewModel)
    {
        var compareDate = Utils.GetMostRelevantDate(newEntryViewModel);
        
        int i = 0;
        while (i < Entries.Count && Utils.GetMostRelevantDate(Entries[i]) > compareDate) i++;
        
        Entries.Insert(i, newEntryViewModel);

        WeakReferenceMessenger.Default.Send(new Messages.PerformAutoSaveMessage());
    }

    [RelayCommand]
    private void ContextMenu_Remove()
    {
        _mainWindowViewModel.RemoveDiary(this);
    }

    [RelayCommand]
    private async Task ContextMenu_Edit()
    {
        Diary? model = await WindowManager.OpenDialogWindowDI<EditDiaryWindow, Diary?, DiaryViewModel>(WindowManager.GetMainWindow(), this);

        if (model == null)
            return;
        
        Name = model.Name;
        
        WeakReferenceMessenger.Default.Send(new Messages.PerformAutoSaveMessage());
    }

    public void RemoveEntry(DiaryEntryViewModel item)
    {
        Entries.Remove(item);
        
        WeakReferenceMessenger.Default.Send(new Messages.PerformAutoSaveMessage());
    }

    public void ResortEntry(DiaryEntryViewModel item)
    {
        Entries.Remove(item);
        InsertEntry(item); // readd entry
    }
}