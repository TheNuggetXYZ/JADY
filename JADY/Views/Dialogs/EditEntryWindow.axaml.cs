using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.ViewModels;
using JADY.Views.Base;

namespace JADY.Views.Dialogs;

public partial class EditEntryWindow : DialogWindow<DiaryEntry>, IDialogInitializable<DiaryEntryViewModel>
{
    public void Initialize(DiaryEntryViewModel data)
    {
        DataContext = data;
        InitializeComponent();
        
        EntryStatus.ItemsSource = Utils.EntryStatusToArray;
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
}