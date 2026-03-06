using D7Task.Abstractions;
using D7Task.Questions;
using D7Task.Models;
using D7Task.Core;

namespace D7Task.Core;

/// <summary>Practice exam showing all answers and grades</summary>
public sealed class PracticeExam : Exam
{
    public PracticeExam(int time, int numberOfQuestions, Subject subject)
        : base(time, numberOfQuestions, subject)
    {
    }

    public override void ShowExam()
    {
        Console.WriteLine("\n========== PRACTICE EXAM ==========");
        Console.WriteLine($"Subject: {Subject.Name}");
        Console.WriteLine($"Time: {Time} minutes");
        Console.WriteLine($"Mode: {Mode}\n");

        int i = 1;
        foreach (var entry in QuestionAnswerDictionary)
        {
            Question question = entry.Key;
            Answer studentAnswer = entry.Value;

            Console.WriteLine($"Q{i}: {question.Header}");
            Console.WriteLine($"Your answer: {studentAnswer}");
            Console.WriteLine($"Correct: {question.CorrectAnswer}");
            Console.WriteLine($"Result: {(question.CheckAnswer(studentAnswer) ? "✓" : "✗")} ({question.Marks} pts)\n");
            i++;
        }

        int totalScore = CorrectExam();
        int totalMarks = 0;
        for (int j = 0; j < Questions.Length; j++)
        {
            if (Questions[j] != null) totalMarks += Questions[j].Marks;
        }

        Console.WriteLine($"========== GRADE ==========");
        Console.WriteLine($"Score: {totalScore}/{totalMarks}");
        Console.WriteLine($"Percentage: {(totalMarks > 0 ? (totalScore * 100.0 / totalMarks) : 0):F2}%\n");
    }

    protected override Exam CreateClone(Question[] clonedQuestions)
    {
        var clone = new PracticeExam(Time, NumberOfQuestions, Subject);
        clone.Questions = clonedQuestions;
        return clone;
    }
}
