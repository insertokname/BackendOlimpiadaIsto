namespace domain.ValueObjects;

public class AnswerList
{
    public List<string> Answers { get; set; } = null!;
    public int CorrectAnswerIndex { get; set; }

    public AnswerList() { }

    public AnswerList(List<string> answers, int correctAnswerIndex)
    {
        Answers = answers;
        if (correctAnswerIndex < 0 || correctAnswerIndex >= answers.Count)
        {
            throw new ArgumentException("Index out of bound of the answers list", nameof(correctAnswerIndex));
        }
        CorrectAnswerIndex = correctAnswerIndex;
    }

}