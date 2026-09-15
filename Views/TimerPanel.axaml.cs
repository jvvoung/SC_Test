using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace SKCTPractice.Views;

public partial class TimerPanel : UserControl
{
    // 기본 시험 시간을 바꾸려면 이 값만 수정합니다.
    public static readonly TimeSpan DefaultDuration = TimeSpan.FromMinutes(15);

    private readonly DispatcherTimer _timer;
    private TimeSpan _remainingTime = DefaultDuration;
    private DateTimeOffset _targetTime;

    public TimerPanel()
    {
        InitializeComponent();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        _timer.Tick += OnTimerTick;

        this.FindControl<Button>("StartButton")!.Click += (_, _) => StartTimer();
        this.FindControl<Button>("PauseButton")!.Click += (_, _) => PauseTimer();
        this.FindControl<Button>("ResetButton")!.Click += (_, _) => ResetTimer();
        UpdateDisplay();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void StartTimer()
    {
        if (_timer.IsEnabled || _remainingTime <= TimeSpan.Zero)
        {
            return;
        }

        _targetTime = DateTimeOffset.UtcNow + _remainingTime;
        _timer.Start();
    }

    private void PauseTimer()
    {
        if (!_timer.IsEnabled)
        {
            return;
        }

        UpdateRemainingTime();
        _timer.Stop();
        UpdateDisplay();
    }

    private void ResetTimer()
    {
        _timer.Stop();
        _remainingTime = DefaultDuration;
        UpdateDisplay();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        UpdateRemainingTime();
        if (_remainingTime <= TimeSpan.Zero)
        {
            _remainingTime = TimeSpan.Zero;
            _timer.Stop();
        }

        UpdateDisplay();
    }

    private void UpdateRemainingTime()
    {
        var exactRemaining = _targetTime - DateTimeOffset.UtcNow;
        _remainingTime = exactRemaining <= TimeSpan.Zero
            ? TimeSpan.Zero
            : TimeSpan.FromSeconds(Math.Ceiling(exactRemaining.TotalSeconds));
    }

    private void UpdateDisplay()
    {
        var timeText = this.FindControl<TextBlock>("TimeText")!;
        var totalSeconds = Math.Max(0, (int)Math.Ceiling(_remainingTime.TotalSeconds));
        timeText.Text = $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
        timeText.Classes.Set("timer-critical", totalSeconds <= 60);
    }
}
