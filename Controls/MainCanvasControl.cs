using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace AvaloniaDemo.Controls;

public class CanvasBox
{
    public string Label { get; set; } = string.Empty;
    public Rect Bounds { get; set; }
    public bool IsSelected { get; set; }
}

public class MainCanvasControl : Control
{
    private readonly List<CanvasBox> _boxes = new();
    private CanvasBox? _selectedBox;
    private Point _dragOffset;
    private bool _isDragging;
    private MouseButton _dragButton;

    public MainCanvasControl()
    {
        // Define a few demo boxes
        _boxes.Add(new CanvasBox { Label = "Module A", Bounds = new Rect(60, 80, 140, 60) });
        _boxes.Add(new CanvasBox { Label = "Module B", Bounds = new Rect(280, 80, 140, 60) });
        _boxes.Add(new CanvasBox { Label = "Module C", Bounds = new Rect(170, 220, 140, 60) });
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // Dark background
        context.FillRectangle(new SolidColorBrush(Color.FromRgb(30, 32, 40)), new Rect(Bounds.Size));

        // Draw connections (lines between boxes — only when at least 3 boxes exist)
        if (_boxes.Count >= 3)
        {
            var linePen = new Pen(new SolidColorBrush(Color.FromRgb(100, 180, 255)), 2);
            DrawConnection(context, linePen, _boxes[0], _boxes[1]);
            DrawConnection(context, linePen, _boxes[1], _boxes[2]);
            DrawConnection(context, linePen, _boxes[0], _boxes[2]);
        }

        // Draw boxes
        foreach (var box in _boxes)
        {
            var fillColor = box.IsSelected
                ? Color.FromRgb(60, 110, 180)
                : Color.FromRgb(50, 55, 70);
            var borderColor = box.IsSelected
                ? Color.FromRgb(120, 180, 255)
                : Color.FromRgb(90, 100, 130);

            context.FillRectangle(new SolidColorBrush(fillColor), box.Bounds, 6);
            context.DrawRectangle(new Pen(new SolidColorBrush(borderColor), 2), box.Bounds, 6);

            // Label text
            var labelBrush = new SolidColorBrush(Color.FromRgb(220, 225, 240));
            var formattedText = new FormattedText(
                box.Label,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                Typeface.Default,
                13,
                labelBrush);

            var textX = box.Bounds.X + (box.Bounds.Width - formattedText.Width) / 2;
            var textY = box.Bounds.Y + (box.Bounds.Height - formattedText.Height) / 2;
            context.DrawText(formattedText, new Point(textX, textY));
        }
    }

    private static void DrawConnection(DrawingContext context, Pen pen, CanvasBox from, CanvasBox to)
    {
        var start = new Point(
            from.Bounds.X + from.Bounds.Width / 2,
            from.Bounds.Y + from.Bounds.Height / 2);
        var end = new Point(
            to.Bounds.X + to.Bounds.Width / 2,
            to.Bounds.Y + to.Bounds.Height / 2);
        context.DrawLine(pen, start, end);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var point = e.GetCurrentPoint(this);
        var pos = point.Position;

        _selectedBox = null;
        foreach (var box in _boxes)
            box.IsSelected = false;

        foreach (var box in _boxes)
        {
            if (box.Bounds.Contains(pos))
            {
                box.IsSelected = true;
                _selectedBox = box;
                _dragOffset = new Point(pos.X - box.Bounds.X, pos.Y - box.Bounds.Y);
                _isDragging = true;
                _dragButton = point.Properties.PointerUpdateKind switch
                {
                    PointerUpdateKind.LeftButtonPressed => MouseButton.Left,
                    PointerUpdateKind.RightButtonPressed => MouseButton.Right,
                    PointerUpdateKind.MiddleButtonPressed => MouseButton.Middle,
                    _ => MouseButton.None
                };
                break;
            }
        }

        InvalidateVisual();
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_isDragging && _selectedBox != null && _dragButton == MouseButton.Left
            && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var pos = e.GetPosition(this);
            _selectedBox.Bounds = new Rect(
                pos.X - _dragOffset.X,
                pos.Y - _dragOffset.Y,
                _selectedBox.Bounds.Width,
                _selectedBox.Bounds.Height);
            InvalidateVisual();
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        _isDragging = false;
    }
}
