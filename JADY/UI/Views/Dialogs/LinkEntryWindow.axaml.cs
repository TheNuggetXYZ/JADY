using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using JADY.Core.Data;
using JADY.Core.DialogInitializableData;
using JADY.Core.Models;
using JADY.Services;
using JADY.UI.Base;
using JADY.ViewModels;

namespace JADY.UI.Views.Dialogs;

public partial class LinkEntryWindow : DialogWindow<DiaryEntry>, IDialogInitializable<LinkEntryInitData>
{
    private readonly IFileAttachmentService _fileAttachmentService;
    private readonly ISaveService _saveService;
    
    private IReadOnlyList<IStorageFile> _rawFileAttachmentList;
    private List<FileAttachment> _fileAttachmentList;

    public LinkEntryWindow(ISaveService saveService, IFileAttachmentService fileAttachmentService)
    {
        _saveService = saveService;
        _fileAttachmentService = fileAttachmentService;
    }

    public void Initialize(LinkEntryInitData data)
    {
        DataContext = data.Entry;
        
        InitializeComponent();
        
        EntryStatus.ItemsSource = new[] {"Note", "End note"};
        EntryStatus.SelectedIndex = data.DefaultToEndNote ? 1 : 0;
        EntryDate.SelectedDate = DateTime.Now;
        EntryDate.CustomDateFormatString = _saveService.Config.CultureInfo.DateTimeFormat.ShortDatePattern;
    }
    
    protected override async Task SubmitAsync(Optional<DiaryEntry> value)
    {
        _fileAttachmentList = await _fileAttachmentService.CreateAttachments(_rawFileAttachmentList);
        
        await base.SubmitAsync(value);
    }

    protected override Optional<DiaryEntry> GetValue()
    {
        return new DiaryEntry
        {
            Category = EntryCategory.Text,
            SubCategory = EntrySubcategory.Text,
            Title = EntryTitle.Text,
            Content = EntryContent.Text,
            LogDate = DateTimeOffset.Now,
            Date = EntryDate.SelectedDate,
            IsHidden = EntryIsHidden.IsChecked ?? false,
            Status = (LinkEntryParameter)EntryStatus.SelectedIndex switch
            {
                LinkEntryParameter.Note => Core.Data.EntryStatus.LinkNote,
                LinkEntryParameter.EndNote => Core.Data.EntryStatus.LinkEndNote,
                _ => throw new ArgumentOutOfRangeException()
            },
            FileAttachments = _fileAttachmentList
        };
    }

    protected override InputElement? FocusedElement() => EntryTitle;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
    
    private async void AttachFiles_OnClick(object? sender, RoutedEventArgs e) => _rawFileAttachmentList = await _fileAttachmentService.SelectAttachments(StorageProvider);
}