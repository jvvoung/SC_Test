namespace SKCTPractice.Models;

public sealed class QuestionItem
{
    public QuestionItem(int number) => Number = number;

    public int Number { get; }
    public int? SelectedAnswer { get; set; }
}
