using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SKCTPractice.Controls;

namespace SKCTPractice.Views;

public partial class MemoDrawingPanel : UserControl
{
    public MemoDrawingPanel()
    {
        InitializeComponent();
        this.FindControl<Button>("MemoTabButton")!.Click += (_, _) => ShowMemo();
        this.FindControl<Button>("DrawingTabButton")!.Click += (_, _) => ShowDrawing();
        this.FindControl<Button>("ClearAllButton")!.Click += ClearAll;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void ShowMemo()
    {
        this.FindControl<TextBox>("MemoTextBox")!.IsVisible = true;
        this.FindControl<Border>("DrawingHost")!.IsVisible = false;
        this.FindControl<Button>("MemoTabButton")!.Classes.Set("active", true);
        this.FindControl<Button>("DrawingTabButton")!.Classes.Set("active", false);
        this.FindControl<TextBox>("MemoTextBox")!.Focus();
    }

    private void ShowDrawing()
    {
        this.FindControl<TextBox>("MemoTextBox")!.IsVisible = false;
        this.FindControl<Border>("DrawingHost")!.IsVisible = true;
        this.FindControl<Button>("MemoTabButton")!.Classes.Set("active", false);
        this.FindControl<Button>("DrawingTabButton")!.Classes.Set("active", true);
    }

    private void ClearAll(object? sender, RoutedEventArgs e)
    {
        this.FindControl<TextBox>("MemoTextBox")!.Clear();
        this.FindControl<DrawingCanvas>("DrawingCanvas")!.ClearStrokes();
    }
}
