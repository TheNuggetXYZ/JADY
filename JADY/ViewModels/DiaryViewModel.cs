using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Core;
using JADY.Core.Models;
using JADY.Factories;
using JADY.Services;
using JADY.Views.Dialogs;

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
    private readonly IWindowService _windowService;

    public DiaryViewModel(Diary diary, MainWindowViewModel mainWindowViewModel, IDiaryEntryViewModelFactory diaryEntryViewModelFactory, IWindowService windowService)
    {
        _diaryEntryViewModelFactory = diaryEntryViewModelFactory;
        _windowService = windowService;
        _mainWindowViewModel = mainWindowViewModel;
        Name = diary.Name;
        Entries = new ObservableCollection<DiaryEntryViewModel>(diary.Entries.OrderByDescending(GetMostRelevantDate).Select(x => diaryEntryViewModelFactory.Create(x, this)));
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
        var compareDate = GetMostRelevantDate(newEntryViewModel);
        
        int i = 0;
        while (i < Entries.Count && GetMostRelevantDate(Entries[i]) > compareDate) i++;
        
        Entries.Insert(i, newEntryViewModel);

        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeCreated());
    }

    [RelayCommand]
    private async Task ContextMenu_Remove()
    {
        await _mainWindowViewModel.RemoveDiary(this);
    }

    [RelayCommand]
    private async Task ContextMenu_Edit()
    {
        Optional<Diary> model = await _windowService.OpenDialogWindowDI<EditDiaryWindow, Diary, DiaryViewModel>(_windowService.GetMainWindow(), this);

        if (!model.HasValue)
            return;
        
        Name = model.Value.Name;
        
        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeCreated());
    }

    public async Task RemoveEntry(DiaryEntryViewModel item)
    {
        Optional<bool> pickedYes = await _windowService.OpenYesNoMessageBox(_windowService.GetMainWindow(), "Are you sure you want to remove this entry?", "Remove entry?");
        if (!pickedYes.HasValue || pickedYes.Value == false) return;
        
        Entries.Remove(item);
        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeCreated());
    }

    public void ResortEntry(DiaryEntryViewModel item)
    {
        Entries.Remove(item);
        InsertEntry(item); // readd entry
    }
    
    private static DateTimeOffset GetMostRelevantDate(DiaryEntryViewModel vm) => vm.Date ?? vm.EndDate ?? vm.LogDate;
    private static DateTimeOffset GetMostRelevantDate(DiaryEntry m) => m.Date ?? m.EndDate ?? m.LogDate;
}