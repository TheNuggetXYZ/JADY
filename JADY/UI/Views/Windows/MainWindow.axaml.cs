using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using JADY.ViewModels;

namespace JADY.UI.Views.Windows;

public partial class MainWindow : Window
{
    private bool _handledUnsavedChanges;
    private TextBox? _searchBox;
    
    public ICommand MinimizeWindowCommand { get; }
    public ICommand MaximizeWindowCommand { get; }
    
    private const int ResizeThickness = 5;

    private readonly IBrush? _windowBorderBackgroundBrush;
    private readonly CornerRadius _windowBorderCornerRadius;
    private readonly CornerRadius _windowBorderClipCornerRadius;

    public MainWindow()
    {
        InitializeComponent();

        _windowBorderBackgroundBrush = PART_WindowBorder.Background;
        _windowBorderCornerRadius = PART_WindowBorder.CornerRadius;
        _windowBorderClipCornerRadius = PART_WindowBorderClip.CornerRadius;

        // Simple commands to bridge the view states
        MinimizeWindowCommand = new RelayCommand(() => WindowState = WindowState.Minimized);
        MaximizeWindowCommand = new RelayCommand(() =>
        {
            WindowState = WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        });
        
        AddHandler(TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == WindowStateProperty && change.NewValue is WindowState state)
        {
            if (state is WindowState.Maximized or WindowState.FullScreen)
            {
                PART_WindowBorder.Background = null;
                PART_WindowBorder.CornerRadius = new CornerRadius();
                PART_WindowBorderClip.ClipToBounds = false;
                PART_WindowBorderClip.CornerRadius = new CornerRadius();
            }
            else
            {
                PART_WindowBorder.Background = _windowBorderBackgroundBrush;
                PART_WindowBorder.CornerRadius = _windowBorderCornerRadius;
                PART_WindowBorderClip.ClipToBounds = true;
                PART_WindowBorderClip.CornerRadius = _windowBorderClipCornerRadius;
            }
        }
            
    }
    
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        Point pos = e.GetPosition(this);
        WindowEdge? edge = GetWindowEdgeAtPosition(pos);

        Cursor = edge switch
        {
            WindowEdge.North => new Cursor(StandardCursorType.TopSide),
            WindowEdge.South => new Cursor(StandardCursorType.BottomSide),
            WindowEdge.West => new Cursor(StandardCursorType.LeftSide),
            WindowEdge.East => new Cursor(StandardCursorType.RightSide),
            WindowEdge.NorthEast => new Cursor(StandardCursorType.TopRightCorner),
            WindowEdge.NorthWest => new Cursor(StandardCursorType.TopLeftCorner),
            WindowEdge.SouthEast => new Cursor(StandardCursorType.BottomRightCorner),
            WindowEdge.SouthWest => new Cursor(StandardCursorType.BottomLeftCorner),
            _ => Cursor.Default
        };
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
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

        // RESIZE
        
        Point pos = e.GetPosition(this);
        WindowEdge? edge = GetWindowEdgeAtPosition(pos);

        if (edge.HasValue && WindowState != WindowState.Maximized) 
            BeginResizeDrag(edge.Value, e);
    }

    private void AppHeader_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // DRAG
        BeginMoveDrag(e);
    }

    private WindowEdge? GetWindowEdgeAtPosition(Point pos)
    {
        bool top = pos.Y < ResizeThickness;
        bool bottom = pos.Y > Bounds.Height - ResizeThickness;
        bool left = pos.X < ResizeThickness;
        bool right = pos.X > Bounds.Width - ResizeThickness;

        // Determine corners first
        if (top && left) return WindowEdge.NorthWest;
        if (top && right) return WindowEdge.NorthEast;
        if (bottom && left) return WindowEdge.SouthWest;
        if (bottom && right) return WindowEdge.SouthEast;

        // Determine flat edges
        if (top) return WindowEdge.North;
        if (bottom) return WindowEdge.South;
        if (left) return WindowEdge.West;
        if (right) return WindowEdge.East;

        return null; // Mouse is inside the window content area
    }
}