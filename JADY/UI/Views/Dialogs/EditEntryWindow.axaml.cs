using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Core.Data;
using JADY.Core.Helpers;
using JADY.Core.Models;
using JADY.UI.Base;
using JADY.ViewModels;

namespace JADY.UI.Views.Dialogs;

public partial class EditEntryWindow : DialogWindow<DiaryEntry>, IDialogInitializable<DiaryEntryViewModel>
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