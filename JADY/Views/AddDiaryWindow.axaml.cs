using Avalonia;
using Avalonia.Interactivity;
using JADY.Models;

namespace JADY.Views;

public partial class AddDiaryWindow : DialogWindowBase<Diary>
{
    public AddDiaryWindow()
    {
        InitializeComponent();
        
        Name.PropertyChanged += NameOnPropertyChanged;
    }
    
    protected override bool CanSubmit() => SubmitButton.IsEnabled;

    protected override Diary GetValue()
    {
        return new Diary()
        {
            Name = Name.Text,
        };
    }

    private void Submit_OnClick(object? sender, RoutedEventArgs e) => TrySubmit();

    private void NameOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        SubmitButton.IsEnabled = !string.IsNullOrWhiteSpace(Name.Text);
    }
}