using System;
using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;

namespace JADY.Views.Behaviors;

public class SmoothHorizontalScrollBehavior : Behavior<ScrollViewer>
{
    public double ScrollFactor { get; set; } = 60.0;
    public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(250);

    private double _targetX;
    private CancellationTokenSource? _cts;

    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject != null)
        {
            _targetX = AssociatedObject.Offset.X; // init _targetX
            AssociatedObject.PointerWheelChanged += OnPointerWheelChanged;
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject != null)
            AssociatedObject.PointerWheelChanged -= OnPointerWheelChanged;
        
        _cts?.Cancel();
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (AssociatedObject == null) return;

        // Cancel animation
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        // Increment the target destination
        _targetX -= e.Delta.Y * ScrollFactor;

        // Clamp target
        double maxScroll = Math.Max(0, AssociatedObject.Extent.Width - AssociatedObject.Viewport.Width);
        _targetX = Math.Clamp(_targetX, 0, maxScroll);

        // Play animation
        var animation = new Animation
        {
            Duration = Duration,
            Easing = new QuadraticEaseOut(),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Setters = { new Setter(ScrollViewer.OffsetProperty, new Vector(_targetX, AssociatedObject.Offset.Y)) },
                    Cue = new Cue(1d)
                }
            }
        };

        animation.RunAsync(AssociatedObject, _cts.Token);
        
        e.Handled = true;
    }
}