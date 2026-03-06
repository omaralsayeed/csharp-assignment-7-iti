using D7Task.Abstractions;
using D7Task.Questions;
using D7Task.Models;
using D7Task.Core;

namespace D7Task.Core;

/// <summary>Final exam hiding correct answers (reveal after completion)</summary>
public sealed class FinalExam : Exam
{
    public FinalExam(int time, int numberOfQuestions, Subject subject)
        : base(time, numberOfQuestions, subject)
    {
    }

    public override void ShowExam()
    {
        Console.WriteLine("\n========== FINAL EXAM ==========");
        Console.WriteLine($"Subject: {Subject.Name}");
        Console.WriteLine($"Time: {Time} minutes");
        Console.WriteLine($"Mode: {Mode}\n");

        int i = 1;
        foreach (var entry in QuestionAnswerDictionary)
        {
            Question question = entry.Key;
            Answer studentAnswer = entry.Value;

            Console.WriteLine($"Q{i}: {question.Header}");
            Console.WriteLine($"Your answer: {studentAnswer}\n");
            i++;
        }

        Console.WriteLine("========== NOTICE ==========");
        Console.WriteLine("Answers revealed after official results.\n");
    }

    protected override Exam CreateClone(Question[] clonedQuestions)
    {
        var clone = new FinalExam(Time, NumberOfQuestions, Subject);
        clone.Questions = clonedQuestions;
        return clone;
    }
}
