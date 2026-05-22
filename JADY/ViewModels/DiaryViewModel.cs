using System;
using System.Collections.Generic;
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
using JADY.UI.Views.Dialogs;

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
    
    public IEnumerable<DiaryEntryViewModel> FilteredEntries => 
        string.IsNullOrWhiteSpace(_filter) ? Entries : Entries.Where(ApplyFilters);

    private readonly IDiaryEntryViewModelFactory _diaryEntryViewModelFactory;
    private readonly IWindowService _windowService;
    private readonly IDiaryService _diaryService;

    private string? _filter;

    public DiaryViewModel(Diary diary, IDiaryEntryViewModelFactory diaryEntryViewModelFactory, IWindowService windowService, IDiaryService diaryService)
    {
        _diaryEntryViewModelFactory = diaryEntryViewModelFactory;
        _windowService = windowService;
        _diaryService = diaryService;
        Name = diary.Name;
        Entries = new ObservableCollection<DiaryEntryViewModel>(diary.Entries.OrderByDescending(GetMostRelevantDate).Select(x => diaryEntryViewModelFactory.Create(x, this)));

        Entries.CollectionChanged += (_, _) => UpdateFilter(); // TODO: handle unsubscribing
    }
    
    /// <returns>
    /// a model using this view model's values.
    /// </returns>
    public Diary GetModel()
    {
        return new()
        {
            Name = Name,
            Entries = [..Entries.Select(x => x.GetModel())]
        };
    }

    private bool ApplyFilters(DiaryEntryViewModel entry)
    {
        if (_filter is null)
            return true;
        
        return (entry.Title?.Contains(_filter, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (entry.Content?.Contains(_filter, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (entry.Category?.Contains(_filter, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (entry.SubCategory?.Contains(_filter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private void UpdateFilter()
    {
        OnPropertyChanged(nameof(FilteredEntries));
    }

    public void SetFilter(string? filter)
    {
        _filter = filter;
        UpdateFilter();
    }
    
    public void RemoveFilter()
    {
        _filter = null;
        UpdateFilter();
    }
    
    public void AddEntry(DiaryEntry model, DiaryEntryViewModel? parentEntry = null)
    {
        var newEntryViewModel = _diaryEntryViewModelFactory.Create(model, this, parentEntry);
        
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
        await _diaryService.RemoveDiary(this);
    }

    [RelayCommand]
    private async Task ContextMenu_Edit()
    {
        var model = await _windowService.OpenDialogWindowDI<EditDiaryWindow, Diary, DiaryViewModel>(_windowService.GetMainWindow(), this);

        if (!model.HasValue)
            return;
        
        Name = model.Value.Name;
        
        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeCreated());
    }

    public async Task RemoveEntry(DiaryEntryViewModel item)
    {
        var pickedYes = await _windowService.OpenYesNoMessageBox(_windowService.GetMainWindow(), "Are you sure you want to remove this entry?", "Remove entry?");
        if (!pickedYes.HasValue || pickedYes.Value == false) return;

        var guid = item.EntryGuid;
        
        Entries.Remove(item);

        CascadeRemoveEntries(guid);

        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeCreated());
    }

    private List<DiaryEntryViewModel> CascadeLookup(Guid guid)
    {
        return Entries.Where(x => x.ParentEntryGuid?.Equals(guid) ?? false).ToList();
    }

    private void CascadeRemoveEntries(Guid guid)
    {
        foreach (var child in CascadeLookup(guid))
            Entries.Remove(child);
    }

    public void CascadeEditEntries(DiaryEntryViewModel entry)
    {
        foreach (var child in CascadeLookup(entry.EntryGuid))
        {
            child.Category = entry.Category;
            child.SubCategory = entry.SubCategory;
        }
    }

    public void ResortEntry(DiaryEntryViewModel item)
    {
        Entries.Remove(item);
        InsertEntry(item); // readd entry
    }
    
    private static DateTimeOffset GetMostRelevantDate(DiaryEntryViewModel vm) => vm.Date ?? vm.EndDate ?? vm.LogDate;
    private static DateTimeOffset GetMostRelevantDate(DiaryEntry m) => m.Date ?? m.EndDate ?? m.LogDate;
}