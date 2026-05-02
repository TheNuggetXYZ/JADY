using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Views;

public partial class EditDiaryWindow : DialogWindow<Diary>, IDialogInitializable<DiaryViewModel>
{
    public void Initialize(DiaryViewModel data)
    {
        DataContext = data;
        
        InitializeComponent();
    }

    protected override Task<bool> CanSubmitAsync() => Task.FromResult(SubmitButton.IsEnabled);

    protected override Diary GetValue()
    {
        return new Diary()
        {
            Name = Name.Text
        };
    }
    
    protected override InputElement? GetFirstFocusableElementOverride() => Name;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
    
    private void NameOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (SubmitButton != null && Name != null)
            SubmitButton.IsEnabled = !string.IsNullOrWhiteSpace(Name.Text);
    }
}