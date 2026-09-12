using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.Services;
using JADY.UI.Base;

namespace JADY.UI.Views.Dialogs;

public partial class AddEntryWindow : DialogWindow<DiaryEntry>
{
    private readonly IFileAttachmentService _fileAttachmentService;
    
    private List<FileAttachment> _fileAttachmentList;
    
    public AddEntryWindow(ISaveService saveService, IFileAttachmentService fileAttachmentService)
    {
        _fileAttachmentService = fileAttachmentService;
        
        InitializeComponent();

        EntryParameter.ItemsSource = new[]{"One time", "Started"};
        EntryDate.SelectedDate = DateTime.Now;
        EntryDate.CustomDateFormatString = saveService.Config.CultureInfo.DateTimeFormat.ShortDatePattern;
    }
    
    protected override Optional<DiaryEntry> GetValue()
    {
        _fileAttachmentService.SaveAttachments(_fileAttachmentList);
        
        return new DiaryEntry()
        {
            Category = EntryCategory.Text,
            SubCategory = EntrySubcategory.Text,
            Title = EntryTitle.Text,
            Content = EntryContent.Text,
            LogDate = DateTimeOffset.Now,
            Date = EntryDate.SelectedDate,
            IsHidden = EntryIsHidden.IsChecked ?? false,
            Status = (NewEntryParameter)EntryParameter.SelectedIndex switch
            {
                NewEntryParameter.OneTime => EntryStatus.OneTime,
                NewEntryParameter.Started => EntryStatus.EventInProgress,
                _ => throw new ArgumentOutOfRangeException(nameof(EntryParameter), EntryParameter, null)
            },
            FileAttachments = _fileAttachmentList
        };
    }
    
    protected override InputElement? FocusedElement() => EntryCategory;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
    
    private async void AttachFiles_OnClick(object? sender, RoutedEventArgs e) => _fileAttachmentList = await _fileAttachmentService.SelectAttachments(StorageProvider);
}