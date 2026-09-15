using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using SKCTPractice.Models;

namespace SKCTPractice.Views;

public partial class QuestionPanel : UserControl
{
    private const int QuestionCount = 20;
    private readonly List<QuestionItem> _questions = new();
    private readonly List<QuestionRowControls> _rows = new();
    private int _currentQuestion = 1;

    public QuestionPanel()
    {
        InitializeComponent();
        BuildQuestionRows();

        this.FindControl<Button>("ResetAnswersButton")!.Click += ResetAnswersAsync;
        UpdateVisualState(scrollIntoView: false);
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void BuildQuestionRows()
    {
        var host = this.FindControl<StackPanel>("QuestionRowsHost")!;

        for (var questionNumber = 1; questionNumber <= QuestionCount; questionNumber++)
        {
            var question = new QuestionItem(questionNumber);
            _questions.Add(question);

            var rowGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("25,*"),
                VerticalAlignment = VerticalAlignment.Center
            };
            rowGrid.Children.Add(new TextBlock
            {
                Text = $"{questionNumber}.",
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Avalonia.Media.Brushes.DimGray,
                FontSize = 12
            });

            var answers = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 0
            };
            Grid.SetColumn(answers, 1);

            var answerButtons = new List<Button>();
            for (var answer = 1; answer <= 5; answer++)
            {
                var button = new Button
                {
                    Content = answer.ToString(),
                    Tag = new AnswerTag(questionNumber, answer)
                };
                button.Classes.Add("answer");
                button.Click += OnAnswerClicked;
                answerButtons.Add(button);
                answers.Children.Add(button);
            }

            rowGrid.Children.Add(answers);
            var border = new Border { Child = rowGrid, Tag = questionNumber };
            border.Classes.Add("question-row");
            border.PointerPressed += OnQuestionRowPressed;
            host.Children.Add(border);
            _rows.Add(new QuestionRowControls(border, answerButtons));
        }
    }

    private void OnAnswerClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: AnswerTag tag })
        {
            return;
        }

        _currentQuestion = tag.QuestionNumber;
        _questions[tag.QuestionNumber - 1].SelectedAnswer = tag.Answer;
        UpdateVisualState(scrollIntoView: true);
        e.Handled = true;
    }

    private void OnQuestionRowPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border { Tag: int questionNumber })
        {
            SetCurrentQuestion(questionNumber);
        }
    }

    private void SetCurrentQuestion(int questionNumber)
    {
        _currentQuestion = Math.Clamp(questionNumber, 1, QuestionCount);
        UpdateVisualState(scrollIntoView: true);
    }

    private async void ResetAnswersAsync(object? sender, RoutedEventArgs e)
    {
        var owner = this.GetVisualRoot() as Window;
        if (owner is null)
        {
            return;
        }

        var dialog = new ConfirmationDialog("1~20번 답안을 모두 초기화하시겠습니까?");
        if (!await dialog.ShowDialog<bool>(owner))
        {
            return;
        }

        foreach (var question in _questions)
        {
            question.SelectedAnswer = null;
        }

        UpdateVisualState(scrollIntoView: false);
    }

    private void UpdateVisualState(bool scrollIntoView)
    {
        var answeredCount = _questions.Count(question => question.SelectedAnswer.HasValue);
        this.FindControl<TextBlock>("ProgressText")!.Text = $"{answeredCount} / {QuestionCount}";

        for (var index = 0; index < _rows.Count; index++)
        {
            var question = _questions[index];
            var controls = _rows[index];
            controls.Row.Classes.Set("current", question.Number == _currentQuestion);

            for (var answerIndex = 0; answerIndex < controls.AnswerButtons.Count; answerIndex++)
            {
                controls.AnswerButtons[answerIndex].Classes.Set(
                    "selected", question.SelectedAnswer == answerIndex + 1);
            }
        }

        if (scrollIntoView)
        {
            _rows[_currentQuestion - 1].Row.BringIntoView();
        }
    }

    private sealed record AnswerTag(int QuestionNumber, int Answer);
    private sealed record QuestionRowControls(Border Row, List<Button> AnswerButtons);
}
