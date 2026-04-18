using System.Threading.Tasks;
using Avalonia;
using Avalonia.Interactivity;
using JADY.Models;

namespace JADY.Views;

public partial class AddDiaryWindow : DialogWindowBase<Diary>
{
    public AddDiaryWindow()
    {
        InitializeComponent();
    }
    
    protected override Task<bool> CanSubmitAsync() => Task.FromResult(SubmitButton.IsEnabled);

    protected override Diary GetValue()
    {
        return new Diary()
        {
            Name = Name.Text,
        };
    }

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();

    private void NameOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (SubmitButton != null && Name != null)
            SubmitButton.IsEnabled = !string.IsNullOrWhiteSpace(Name.Text);
    }
}