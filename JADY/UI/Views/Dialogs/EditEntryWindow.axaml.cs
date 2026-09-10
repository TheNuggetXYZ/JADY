using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using JADY.Core.Data;
using JADY.Core.Helpers;
using JADY.Core.Models;
using JADY.Services;
using JADY.UI.Base;
using JADY.ViewModels;

namespace JADY.UI.Views.Dialogs;

public partial class EditEntryWindow : DialogWindow<DiaryEntry>, IDialogInitializable<DiaryEntryViewModel>
{
    private readonly IFileAttachmentService _fileAttachmentService;
    private readonly ISaveService _saveService;
    
    private IReadOnlyList<IStorageFile> _rawFileAttachmentList;
    private List<FileAttachment> _fileAttachmentList;

    public EditEntryWindow(ISaveService saveService, IFileAttachmentService fileAttachmentService)
    {
        _saveService = saveService;
        _fileAttachmentService = fileAttachmentService;
    }

    public void Initialize(DiaryEntryViewModel data)
    {
        DataContext = data;
        InitializeComponent();

        // Allow to go from link to normal entry, but disallow the opposite
        if (!EntryStatusExtensions.IsLink(data.Status))
            EntryStatus.ItemsSource = EntryStatusExtensions.DisplayValuesNoLink;
        else
            EntryStatus.ItemsSource = EntryStatusExtensions.DisplayValues;

        if (!EntryStatusExtensions.IsEvent(data.Status))
        {
            EntryEndDate.IsVisible = false;
            DatePickerGrid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        }
        else
        {
            EntryEndDate.IsVisible = true;
            DatePickerGrid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        }

        if (EntryStatusExtensions.IsEnded(data.Status) && data.WasEndedByLinking)
        {
            EntryStatus.IsEnabled = false;
            EntryEndDate.IsEnabled = false;
        }
        else
        {
            EntryStatus.IsEnabled = true;
            EntryEndDate.IsEnabled = true;
        }
        
        EntryDate.CustomDateFormatString = _saveService.Config.CultureInfo.DateTimeFormat.ShortDatePattern;
        EntryEndDate.CustomDateFormatString = _saveService.Config.CultureInfo.DateTimeFormat.ShortDatePattern;

        _fileAttachmentList = data.FileAttachments;
    }

    protected override async Task SubmitAsync(Optional<DiaryEntry> value)
    {
        if (_rawFileAttachmentList is not { Count: 0})
            _fileAttachmentList = await _fileAttachmentService.CreateAttachments(_rawFileAttachmentList);
        
        await base.SubmitAsync(value);
    }
    
    protected override Optional<DiaryEntry> GetValue()
    {
        return new DiaryEntry()
        {
            Status = (EntryStatus)EntryStatus.SelectedIndex,
            Date = EntryDate.SelectedDate,
            EndDate = EntryEndDate.SelectedDate,
            Category = EntryCategory.Text,
            SubCategory = EntrySubcategory.Text,
            Title = EntryTitle.Text,
            Content = EntryContent.Text,
            IsHidden = EntryIsHidden.IsChecked ?? false,
            FileAttachments = _fileAttachmentList
        };
    }
    
    protected override InputElement? FocusedElement() => EntryCategory;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
    
    private async void AttachFiles_OnClick(object? sender, RoutedEventArgs e) => _rawFileAttachmentList = await _fileAttachmentService.SelectAttachments(StorageProvider);

    private void EntryStatus_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (EntryStatus is null) return;
        
        if (EntryStatusExtensions.IsLink((EntryStatus)EntryStatus.SelectedIndex))
        {
            if (DataContext is not DiaryEntryViewModel entry)
                return;
            
            EntryCategory.Text = entry.Category;
            EntrySubcategory.Text = entry.SubCategory;
            
            EntryCategory.IsReadOnly = true;
            EntrySubcategory.IsReadOnly = true;
        }
        else
        {
            EntryCategory.IsReadOnly = false;
            EntrySubcategory.IsReadOnly = false;
        }
    }
}