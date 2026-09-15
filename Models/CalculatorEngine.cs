using System;
using System.Globalization;

namespace SKCTPractice.Models;

public sealed class CalculatorEngine
{
    private const int MaxInputLength = 28;
    private decimal? _previousValue;
    private string? _pendingOperator;
    private bool _replaceInput;
    private bool _isError;

    public string CurrentInput { get; private set; } = "0";
    public string Expression { get; private set; } = string.Empty;

    public void InputDigit(string digit)
    {
        if (digit is not ("0" or "00" or "1" or "2" or "3" or "4" or "5" or "6" or "7" or "8" or "9"))
        {
            return;
        }

        RecoverFromError();
        if (_replaceInput)
        {
            CurrentInput = "0";
            _replaceInput = false;
        }

        if (CurrentInput.Length >= MaxInputLength)
        {
            return;
        }

        if (CurrentInput == "0")
        {
            CurrentInput = digit.TrimStart('0');
            if (CurrentInput.Length == 0)
            {
                CurrentInput = "0";
            }
        }
        else if (CurrentInput == "-0")
        {
            var significant = digit.TrimStart('0');
            CurrentInput = significant.Length == 0 ? "-0" : $"-{significant}";
        }
        else
        {
            CurrentInput += digit;
        }
    }

    public void InputDecimal()
    {
        RecoverFromError();
        if (_replaceInput)
        {
            CurrentInput = "0";
            _replaceInput = false;
        }

        if (!CurrentInput.Contains('.', StringComparison.Ordinal))
        {
            CurrentInput += ".";
        }
    }

    public void InputOperator(string operation)
    {
        if (operation is not ("+" or "-" or "×" or "÷"))
        {
            return;
        }

        if (_isError)
        {
            Clear();
        }

        if (_previousValue.HasValue && _pendingOperator is not null && !_replaceInput)
        {
            if (!TryApply(_previousValue.Value, ParseCurrent(), _pendingOperator, out var result))
            {
                SetError();
                return;
            }

            CurrentInput = Format(result);
            _previousValue = result;
        }
        else if (!_previousValue.HasValue)
        {
            _previousValue = ParseCurrent();
        }

        _pendingOperator = operation;
        Expression = $"{Format(_previousValue!.Value)} {operation}";
        _replaceInput = true;
    }

    public void Calculate()
    {
        if (_isError || !_previousValue.HasValue || _pendingOperator is null)
        {
            return;
        }

        var rightValue = ParseCurrent();
        var leftValue = _previousValue.Value;
        var operation = _pendingOperator;
        if (!TryApply(leftValue, rightValue, operation, out var result))
        {
            SetError();
            return;
        }

        Expression = $"{Format(leftValue)} {operation} {Format(rightValue)} =";
        CurrentInput = Format(result);
        _previousValue = null;
        _pendingOperator = null;
        _replaceInput = true;
    }

    public void ToggleNegative()
    {
        if (_isError)
        {
            return;
        }

        if (CurrentInput.StartsWith("-", StringComparison.Ordinal))
        {
            CurrentInput = CurrentInput[1..];
        }
        else if (CurrentInput != "0")
        {
            CurrentInput = $"-{CurrentInput}";
        }
    }

    public void Backspace()
    {
        if (_isError)
        {
            Clear();
            return;
        }

        if (_replaceInput)
        {
            return;
        }

        CurrentInput = CurrentInput.Length <= 1 || (CurrentInput.Length == 2 && CurrentInput[0] == '-')
            ? "0"
            : CurrentInput[..^1];
    }

    public void Clear()
    {
        CurrentInput = "0";
        Expression = string.Empty;
        _previousValue = null;
        _pendingOperator = null;
        _replaceInput = false;
        _isError = false;
    }

    private void RecoverFromError()
    {
        if (_isError)
        {
            Clear();
        }
    }

    private decimal ParseCurrent()
    {
        return decimal.TryParse(CurrentInput, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            ? value
            : 0m;
    }

    private static bool TryApply(decimal left, decimal right, string operation, out decimal result)
    {
        try
        {
            result = operation switch
            {
                "+" => left + right,
                "-" => left - right,
                "×" => left * right,
                "÷" when right != 0 => left / right,
                _ => 0m
            };

            return operation != "÷" || right != 0;
        }
        catch (OverflowException)
        {
            result = 0m;
            return false;
        }
    }

    private void SetError()
    {
        CurrentInput = "Error";
        Expression = string.Empty;
        _previousValue = null;
        _pendingOperator = null;
        _replaceInput = true;
        _isError = true;
    }

    private static string Format(decimal value) => value.ToString("G29", CultureInfo.InvariantCulture);
}
