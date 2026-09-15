using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SKCTPractice;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnOpened(object? sender, EventArgs e)
    {
        var screen = Screens.Primary;
        if (screen is null)
        {
            return;
        }

        var area = screen.WorkingArea;
        var scaling = screen.Scaling;
        var availableHeight = area.Height / scaling;
        Height = Math.Min(950, availableHeight);

        var physicalWidth = (int)Math.Ceiling(Width * scaling);
        Position = new PixelPoint(area.Right - physicalWidth, area.Y);
    }
}
