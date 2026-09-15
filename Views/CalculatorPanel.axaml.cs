using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SKCTPractice.Models;

namespace SKCTPractice.Views;

public partial class CalculatorPanel : UserControl
{
    private readonly CalculatorEngine _calculator = new();

    public CalculatorPanel()
    {
        InitializeComponent();
        PointerPressed += (_, _) => Focus();
        KeyDown += OnCalculatorKeyDown;
        TextInput += OnCalculatorTextInput;
        UpdateDisplay();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnCalculatorButtonClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Content: string input })
        {
            ProcessInput(input);
            Focus();
        }
    }

    private void OnCalculatorTextInput(object? sender, TextInputEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        var handled = false;
        foreach (var character in e.Text)
        {
            var input = character switch
            {
                '*' => "×",
                '/' => "÷",
                ',' => ".",
                _ => character.ToString()
            };

            if ("0123456789.+-×÷".Contains(input))
            {
                ProcessInput(input);
                handled = true;
            }
        }

        e.Handled = handled;
    }

    private void OnCalculatorKeyDown(object? sender, KeyEventArgs e)
    {
        string? input = e.Key switch
        {
            Key.NumPad0 => "0",
            Key.NumPad1 => "1",
            Key.NumPad2 => "2",
            Key.NumPad3 => "3",
            Key.NumPad4 => "4",
            Key.NumPad5 => "5",
            Key.NumPad6 => "6",
            Key.NumPad7 => "7",
            Key.NumPad8 => "8",
            Key.NumPad9 => "9",
            Key.Add => "+",
            Key.Subtract => "-",
            Key.Multiply => "×",
            Key.Divide => "÷",
            Key.Decimal => ".",
            Key.Enter => "=",
            Key.Back => "←",
            Key.Escape or Key.Delete => "C",
            _ => null
        };

        if (input is null)
        {
            return;
        }

        ProcessInput(input);
        e.Handled = true;
    }

    private void ProcessInput(string input)
    {
        switch (input)
        {
            case "0" or "1" or "2" or "3" or "4" or "5" or "6" or "7" or "8" or "9" or "00":
                _calculator.InputDigit(input);
                break;
            case "+" or "-" or "×" or "÷":
                _calculator.InputOperator(input);
                break;
            case ".":
                _calculator.InputDecimal();
                break;
            case "NEG":
                _calculator.ToggleNegative();
                break;
            case "←":
                _calculator.Backspace();
                break;
            case "C":
                _calculator.Clear();
                break;
            case "=":
                _calculator.Calculate();
                break;
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        this.FindControl<TextBlock>("ExpressionText")!.Text = _calculator.Expression;
        this.FindControl<TextBlock>("DisplayText")!.Text = _calculator.CurrentInput;
    }
}
