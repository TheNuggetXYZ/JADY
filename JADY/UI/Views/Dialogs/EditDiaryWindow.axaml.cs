using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Core.Models;
using JADY.UI.Base;
using JADY.ViewModels;

namespace JADY.UI.Views.Dialogs;

public partial class EditDiaryWindow : DialogWindow<Diary>, IDialogInitializable<DiaryViewModel>
{
    public void Initialize(DiaryViewModel data)
    {
        DataContext = data;
        
        InitializeComponent();
    }

    protected override Task<bool> CanSubmitAsync() => Task.FromResult<bool>(SubmitButton.IsEnabled);

    protected override Optional<Diary> GetValue()
    {
        return new Diary()
        {
            Name = DiaryName.Text
        };
    }
    
    protected override InputElement? FocusedElement() => DiaryName;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
    
    private void DiaryNameOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (SubmitButton != null && DiaryName != null)
            SubmitButton.IsEnabled = !string.IsNullOrWhiteSpace(DiaryName.Text);
    }
}