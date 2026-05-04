using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.ViewModels;

namespace JADY.Views.Windows;

public partial class MainWindow : Window
{
    private bool _handledUnsavedChanges;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        if (_handledUnsavedChanges)
        {
            return;
        }
        
        e.Cancel = true;
        
        if (DataContext is MainWindowViewModel vm)
        {
            bool cancel = await vm.OnClosing();
            
            if (!cancel)
            {
                _handledUnsavedChanges = true;
                Close();
            }
        }
    }
}