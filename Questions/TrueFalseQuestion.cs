using D7Task.Abstractions;
using D7Task.Models;

namespace D7Task.Questions;

/// <summary>True/False question with only 2 answers</summary>
public sealed class TrueFalseQuestion : Question
{
    public TrueFalseQuestion(string header, string body, int marks, bool isTrueCorrect)
        : base(header, body, marks, CreateAnswers(), isTrueCorrect ? new Answer(1, "True") : new Answer(2, "False"))
    {
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

    private static AnswerList CreateAnswers()
    {
        var list = new AnswerList(2);
        list.Add(new Answer(1, "True"));
        list.Add(new Answer(2, "False"));
        return list;
    }
}
