using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Models;

namespace JADY.Views;

public partial class EditDiaryWindow : DialogWindowBase<Diary>
{
    public EditDiaryWindow()
    {
        InitializeComponent();
    }
    
    //TODO: Add same name check as in AddDiaryWindow.axaml.cs

    protected override Diary GetValue()
    {
        return new Diary()
        {
            Name = Name.Text
        };
    }

    private void Submit_OnClick(object? sender, RoutedEventArgs e) => TrySubmit();
}