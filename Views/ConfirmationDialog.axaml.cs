using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SKCTPractice.Views;

public partial class ConfirmationDialog : Window
{
    public ConfirmationDialog() : this(string.Empty)
    {
    }

    public ConfirmationDialog(string message)
    {
        InitializeComponent();
        this.FindControl<TextBlock>("MessageText")!.Text = message;
        this.FindControl<Button>("YesButton")!.Click += (_, _) => Close(true);
        this.FindControl<Button>("NoButton")!.Click += (_, _) => Close(false);
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
