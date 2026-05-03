using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Core.Models;
using JADY.Views.Base;

namespace JADY.Views.Dialogs;

public partial class AddDiaryWindow : DialogWindow<Diary>
{
    public AddDiaryWindow()
    {
        InitializeComponent();
    }
    
    protected override Task<bool> CanSubmitAsync() => Task.FromResult<bool>(SubmitButton.IsEnabled);

    protected override Optional<Diary> GetValue()
    {
        return new Diary()
        {
            Name = Name.Text,
        };
    }

    protected override InputElement? FocusedElement() => Name;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();

    private void NameOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (SubmitButton != null && Name != null)
            SubmitButton.IsEnabled = !string.IsNullOrWhiteSpace(Name.Text);
    }
}