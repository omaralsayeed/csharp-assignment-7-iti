using D7Task.Abstractions;
using D7Task.Models;

namespace D7Task.Questions;

/// <summary>Multiple choice question - select one answer</summary>
public sealed class ChooseOneQuestion : Question
{
    public ChooseOneQuestion(string header, string body, int marks, AnswerList answers, Answer correctAnswer)
        : base(header, body, marks, answers, correctAnswer)
    {
        if (answers.GetById(correctAnswer.Id) is null)
            throw new ArgumentException("Correct answer must be in list.", nameof(correctAnswer));
    }

    public override void Display()
    {
        Console.WriteLine(ToString());
        for (int i = 0; i < Answers.Count; i++)
        {
            Console.WriteLine(Answers[i]);
        }
    }

    public override bool CheckAnswer(Answer studentAnswer)
    {
        ArgumentNullException.ThrowIfNull(studentAnswer);
        return studentAnswer.Id == CorrectAnswer.Id;
    }
}
