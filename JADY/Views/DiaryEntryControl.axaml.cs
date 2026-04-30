using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using JADY.ViewModels;

namespace JADY.Views;

public partial class DiaryEntryControl : UserControl
{
    public DiaryEntryControl()
    {
        InitializeComponent();
        
        DataContextChanged += OnDataContextChanged;
    }

    // Fix animations, that are undesired when this object gets recycled/created, taking place when switching between diaries.
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        // temporarily remove transitions
        var transitions = ExpandAngle.Transitions;
        ExpandAngle.Transitions = null;

        // force ui update
        ExpandAngle.InvalidateVisual();

        ExpandAngle.Transitions = transitions;
    }

    private void Control_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DiaryEntryViewModel vm)
            vm.IsExpanded = !vm.IsExpanded;
    }
}