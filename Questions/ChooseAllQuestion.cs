using D7Task.Abstractions;
using D7Task.Models;

namespace D7Task.Questions;

/// <summary>Multiple choice question - select all correct answers</summary>
public sealed class ChooseAllQuestion : Question
{
    private readonly int[] _correctIds;

    public ChooseAllQuestion(string header, string body, int marks, AnswerList answers, int[] correctIds)
        : base(header, body, marks, answers, new MultiSelectAnswer(9999, "Multiple Correct", correctIds))
    {
        if (correctIds is null || correctIds.Length == 0)
            throw new ArgumentException("Correct ids required.", nameof(correctIds));

        _correctIds = NormalizeIds(correctIds);
        ValidateIdsExist(answers, _correctIds);
    }

    public override void Display()
    {
        Console.WriteLine(ToString());
        for (int i = 0; i < Answers.Count; i++)
        {
            Console.WriteLine(Answers[i]);
        }
        Console.WriteLine("Select multiple answers.");
    }

    public override bool CheckAnswer(Answer studentAnswer)
    {
        ArgumentNullException.ThrowIfNull(studentAnswer);

        if (studentAnswer is not MultiSelectAnswer selected) return false;

        var submitted = NormalizeIds(selected.SelectedIds);
        if (submitted.Length != _correctIds.Length) return false;

        for (int i = 0; i < _correctIds.Length; i++)
        {
            if (_correctIds[i] != submitted[i]) return false;
        }

        return true;
    }

    /// <summary>Sort and remove duplicates from id array</summary>
    private static int[] NormalizeIds(int[] ids)
    {
        var copy = new int[ids.Length];
        Array.Copy(ids, copy, ids.Length);
        Array.Sort(copy);

        int uniqueCount = 1;
        for (int i = 1; i < copy.Length; i++)
        {
            if (copy[i] != copy[i - 1])
            {
                copy[uniqueCount++] = copy[i];
            }
        }

        var unique = new int[uniqueCount];
        Array.Copy(copy, unique, uniqueCount);
        return unique;
    }

    private static void ValidateIdsExist(AnswerList answers, int[] correctIds)
    {
        for (int i = 0; i < correctIds.Length; i++)
        {
            if (answers.GetById(correctIds[i]) is null)
                throw new ArgumentException($"Answer {correctIds[i]} not found.");
        }
    }
}
