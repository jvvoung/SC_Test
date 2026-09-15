using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace SKCTPractice.Controls;

public sealed class DrawingCanvas : Control
{
    private readonly List<List<Point>> _strokes = new();
    private List<Point>? _activeStroke;
    private static readonly Pen StrokePen = new(Brushes.Black, 2.5, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round);

    public DrawingCanvas()
    {
        ClipToBounds = true;
        Cursor = new Cursor(StandardCursorType.Cross);
    }

    public void ClearStrokes()
    {
        _strokes.Clear();
        _activeStroke = null;
        InvalidateVisual();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var point = e.GetCurrentPoint(this);
        if (!point.Properties.IsLeftButtonPressed)
        {
            return;
        }

        _activeStroke = new List<Point> { point.Position };
        _strokes.Add(_activeStroke);
        e.Pointer.Capture(this);
        e.Handled = true;
        InvalidateVisual();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_activeStroke is null)
        {
            return;
        }

        var point = e.GetCurrentPoint(this);
        if (!point.Properties.IsLeftButtonPressed)
        {
            FinishStroke(e.Pointer);
            return;
        }

        _activeStroke.Add(point.Position);
        e.Handled = true;
        InvalidateVisual();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_activeStroke is not null)
        {
            _activeStroke.Add(e.GetPosition(this));
            FinishStroke(e.Pointer);
            e.Handled = true;
            InvalidateVisual();
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        _activeStroke = null;
        base.OnPointerCaptureLost(e);
    }

    private void FinishStroke(IPointer pointer)
    {
        _activeStroke = null;
        pointer.Capture(null);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.FillRectangle(Brushes.White, Bounds);

        foreach (var stroke in _strokes)
        {
            if (stroke.Count == 1)
            {
                var point = stroke[0];
                context.DrawEllipse(Brushes.Black, null, point, 1.25, 1.25);
                continue;
            }

            for (var index = 1; index < stroke.Count; index++)
            {
                context.DrawLine(StrokePen, stroke[index - 1], stroke[index]);
            }
        }
    }
}
