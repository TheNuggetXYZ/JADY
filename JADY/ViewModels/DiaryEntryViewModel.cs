using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Backend;
using JADY.Models;
using JADY.Views;
using DiaryEntry = JADY.Models.DiaryEntry;

namespace JADY.ViewModels;

public partial class DiaryEntryViewModel : SaveDependentViewModel
{
    [NotifyPropertyChangedFor(nameof(ShowEndEventInContextMenu))]
    [NotifyPropertyChangedFor(nameof(GetStatusDisplayName))]
    [ObservableProperty] private Utils.DiaryEntryStatus _status;

    /// <summary>
    /// The date at the time the entry was added.
    /// </summary>
    [NotifyPropertyChangedFor(nameof(LogDateDisplayName), nameof(FullDateTooltip), nameof(DisplayDate))]
    [ObservableProperty] private DateTimeOffset _logDate;

    /// <summary>
    /// The start date of an event or a date of a one time entry.
    /// </summary>
    [NotifyPropertyChangedFor(nameof(DateDisplayName), nameof(FullDateTooltip), nameof(DisplayDate))]
    [ObservableProperty] private DateTimeOffset? _date;

    /// <summary>
    /// The end date of an event. Is useless for a one time entry.
    /// </summary>
    [NotifyPropertyChangedFor(nameof(EndDateDisplayName), nameof(FullDateTooltip), nameof(DisplayDate))]
    [ObservableProperty] private DateTimeOffset? _endDate;

    /// <summary>
    /// E.g. Game/Anime/Misc
    /// </summary>
    [ObservableProperty] private string? _category;
    
    /// <summary>
    /// E.g. Factorio/OPM/null
    /// </summary>
    [ObservableProperty] private string? _subCategory;

    /// <summary>
    /// E.g. First time playing Factorio
    /// </summary>
    [ObservableProperty] private string? _title;

    /// <summary>
    /// E.g. I started playing Factorio for the first time on the ... version and its ...
    /// </summary>
    [ObservableProperty] private string? _content;

    /// <summary>
    /// Used if you don't want this entry to appear unless you toggle on a setting to show hidden entries.
    /// </summary>
    [NotifyPropertyChangedFor(nameof(IsCurrentlyVisible))]
    [ObservableProperty] private bool _isHidden;
    
    /// <summary>
    /// E.g. [I automated red science, I finally destroyed those biter nests, ...]
    /// </summary>
    public ObservableCollection<DiarySubEntryViewModel> SubEntries { get; set; }// DiaryEntryViewModel
    
    [ObservableProperty] private bool _isExpanded;

    public string GetStatusDisplayName
    {
        get
        {
            return Status switch
            {
                Utils.DiaryEntryStatus.OneTime => "One time event",
                Utils.DiaryEntryStatus.InProgress => "In progress event",
                Utils.DiaryEntryStatus.Completed => "Completed event",
                Utils.DiaryEntryStatus.Dropped => "Dropped event",
                _ => throw new ArgumentOutOfRangeException(nameof(Status), Status, null)
            };
        }
    }

    public bool ShowEndEventInContextMenu => Status == Utils.DiaryEntryStatus.InProgress;

    [SaveDependent]
    public bool IsCurrentlyVisible => !IsHidden || Saves.JadySave.Settings.ShowHiddenEntries;
    
    [SaveDependent]
    public string LogDateDisplayName => LogDate.Date.ToString("d", Saves.JadySave.Settings.CultureInfo);

    [SaveDependent]
    public string? DateDisplayName => Date?.Date.ToString("d", Saves.JadySave.Settings.CultureInfo);
    
    [SaveDependent]
    public string? EndDateDisplayName => EndDate?.Date.ToString("d", Saves.JadySave.Settings.CultureInfo);

    [SaveDependent]
    public string DisplayDate => DateDisplayName ?? EndDateDisplayName ?? LogDateDisplayName;

    [SaveDependent]
    public string? FullDateTooltip =>
        $"Log date: {LogDateDisplayName}\n" +
        $"Date: {DateDisplayName ?? "none"}\n" +
        $"End date: {EndDateDisplayName ?? "none"}";
    
    
    private readonly DiaryViewModel _diaryViewModel;

    public DiaryEntryViewModel(DiaryEntry diaryEntry, DiaryViewModel diaryViewModel)
    {
        _diaryViewModel = diaryViewModel;
        Status = diaryEntry.Status;
        LogDate = diaryEntry.LogDate;
        Date = diaryEntry.Date;
        EndDate = diaryEntry.EndDate;
        Category = diaryEntry.Category;
        SubCategory = diaryEntry.SubCategory;
        Title =  diaryEntry.Title;
        Content = diaryEntry.Content;
        IsHidden = diaryEntry.IsHidden;
        SubEntries = MVMConverter.ConvertModels(diaryEntry.SubEntries, this);
    }
    
    /// <returns>
    /// a model using this view model's values.
    /// </returns>
    public DiaryEntry GetModel()
    {
        return new()
        {
            Status = Status,
            LogDate = LogDate,
            Date = Date,
            EndDate = EndDate,
            Category = Category,
            SubCategory = SubCategory,
            Title = Title,
            Content = Content,
            IsHidden = IsHidden,
            SubEntries = MVMConverter.ConvertViewModels(SubEntries)
        };
    }
    
    [RelayCommand]
    private void ContextMenu_Remove()
    {
        _diaryViewModel.RemoveEntry(this);
    }
    
    [RelayCommand]
    private async Task ContextMenu_Edit()
    {
        DiaryEntry? diaryEntry = await WindowManager.OpenDialogWindow<EditEntryWindow, DiaryEntry?>(WindowManager.GetMainWindow(), this);

        if (diaryEntry == null)
            return;
        
        Status = diaryEntry.Status;
        Date = diaryEntry.Date;
        EndDate = diaryEntry.EndDate;
        Category = diaryEntry.Category;
        SubCategory = diaryEntry.SubCategory;
        Title = diaryEntry.Title;
        Content = diaryEntry.Content;
        IsHidden = diaryEntry.IsHidden;

        _diaryViewModel.ResortEntry(this);
    }

    [RelayCommand]
    private async Task ContextMenu_End()
    {
        if (!ShowEndEventInContextMenu)
            return;
        
        DiaryEntry? diaryEntry = await WindowManager.OpenDialogWindow<EndEntryWindow, DiaryEntry?>(WindowManager.GetMainWindow(), this);
        
        if (diaryEntry == null)
            return;
        
        EndDate = diaryEntry.EndDate;
        Status = diaryEntry.Status;
        
        _diaryViewModel.ResortEntry(this);
    }
    
    public void RemoveSubentry(DiarySubEntryViewModel item)
    {
        SubEntries.Remove(item);
        WeakReferenceMessenger.Default.Send(new Messages.PerformAutoSaveMessage());
    }
}
