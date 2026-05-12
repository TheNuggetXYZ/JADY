using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using JADY.ViewModels;

namespace JADY.UI.Views.Windows;

public partial class MainWindow : Window
{
    private bool _handledUnsavedChanges;
    private TextBox? _searchBox;
    
    public MainWindow()
    {
        InitializeComponent();
        
        AddHandler(TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
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
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape)
        {
            if (_searchBox is null)
                return;
                
            _searchBox.Text = string.Empty;
        }

        if (e is { Key: Key.F, KeyModifiers: KeyModifiers.Control })
        {
            if (_searchBox is null)
                return;
            
            _searchBox.Focus();
            
            if (DataContext is MainWindowViewModel vm)
                vm.IsEntrySearchBarVisible = true;
        }
    }
    
    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text))
            return;

        if (DataContext is MainWindowViewModel vm && _searchBox is {IsFocused: false})
        {
            vm.IsEntrySearchBarVisible = true;

            Dispatcher.UIThread.Post(() =>
            {
                _searchBox.Focus();
                _searchBox.Text += e.Text;
                _searchBox.CaretIndex = _searchBox.Text.Length;
            });
        }
    }

    private void SearchBox_OnAttachedToLogicalTree(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        _searchBox = sender as TextBox;
    }

    private void SearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_searchBox is not null && string.IsNullOrWhiteSpace(_searchBox.Text))
        {
            if (DataContext is MainWindowViewModel vm)
                vm.IsEntrySearchBarVisible = false;
        }
    }
}