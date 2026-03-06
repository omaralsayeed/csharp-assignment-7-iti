using D7Task.Models;

namespace D7Task.Abstractions;

/// <summary>Base question class with answer checking logic</summary>
public abstract class Question
{
    public string Header { get; }
    public string Body { get; }
    public int Marks { get; }
    public AnswerList Answers { get; }
    public Answer CorrectAnswer { get; protected set; }

    protected Question(string header, string body, int marks, AnswerList answers, Answer correctAnswer)
    {
        if (string.IsNullOrWhiteSpace(header))
            throw new ArgumentException("Header required.", nameof(header));

        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Body required.", nameof(body));

        if (marks <= 0)
            throw new ArgumentOutOfRangeException(nameof(marks));

        Header = header;
        Body = body;
        Answers = answers ?? throw new ArgumentNullException(nameof(answers));
        CorrectAnswer = correctAnswer ?? throw new ArgumentNullException(nameof(correctAnswer));
        Marks = marks;
    }

    /// <summary>Display question and its answers</summary>
    public abstract void Display();

    /// <summary>Check if student answer is correct</summary>
    public abstract bool CheckAnswer(Answer studentAnswer);

    public override string ToString()
    {
        return $"[{Header}] {Body} (Marks: {Marks})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Question other) return false;

        return string.Equals(Header, other.Header, StringComparison.Ordinal)
               && string.Equals(Body, other.Body, StringComparison.Ordinal)
               && Marks == other.Marks
               && Equals(CorrectAnswer, other.CorrectAnswer);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Header, Body, Marks, CorrectAnswer);
    }
}
