using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Core.Data;
using JADY.Core.Helpers;
using JADY.Core.Models;
using JADY.Services;
using JADY.UI.Base;
using JADY.ViewModels;

namespace JADY.UI.Views.Dialogs;

public partial class EditEntryWindow(ISaveService saveService) : DialogWindow<DiaryEntry>, IDialogInitializable<DiaryEntryViewModel>
{
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
        
        EntryDate.CustomDateFormatString = saveService.Config.CultureInfo.DateTimeFormat.ShortDatePattern;
        EntryEndDate.CustomDateFormatString = saveService.Config.CultureInfo.DateTimeFormat.ShortDatePattern;
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
        };
    }
    
    protected override InputElement? FocusedElement() => EntryCategory;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();

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