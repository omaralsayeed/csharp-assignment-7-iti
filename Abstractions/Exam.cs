using D7Task.Questions;
using D7Task.Models;
using D7Task.Core;

namespace D7Task.Abstractions;

/// <summary>Base exam class with lifecycle management and grading</summary>
public abstract class Exam : ICloneable, IComparable<Exam>
{
    public int Time { get; protected set; }
    public int NumberOfQuestions { get; protected set; }
    public Question[] Questions { get; protected set; }
    public Dictionary<Question, Answer> QuestionAnswerDictionary { get; protected set; }
    public Subject Subject { get; protected set; }
    public ExamMode Mode { get; protected set; }

    public event ExamStartedHandler? ExamStarted;

    protected Exam(int time, int numberOfQuestions, Subject subject)
    {
        if (time <= 0)
            throw new ArgumentOutOfRangeException(nameof(time));

        if (numberOfQuestions <= 0)
            throw new ArgumentOutOfRangeException(nameof(numberOfQuestions));

        Time = time;
        NumberOfQuestions = numberOfQuestions;
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        Questions = new Question[numberOfQuestions];
        QuestionAnswerDictionary = new Dictionary<Question, Answer>();
        Mode = ExamMode.Queued;
    }

    /// <summary>Display exam results (behavior differs by exam type)</summary>
    public abstract void ShowExam();

    /// <summary>Start exam, trigger events, notify students</summary>
    public virtual void Start()
    {
        Mode = ExamMode.Starting;
        OnExamStarted();
    }

    /// <summary>Mark exam as finished</summary>
    public virtual void Finish()
    {
        Mode = ExamMode.Finished;
    }

    /// <summary>Auto-grade exam by checking each answer</summary>
    public int CorrectExam()
    {
        int score = 0;
        foreach (var kvp in QuestionAnswerDictionary)
        {
            if (kvp.Key.CheckAnswer(kvp.Value))
                score += kvp.Key.Marks;
        }
        return score;
    }

    /// <summary>Compare exams: first by time, then by question count</summary>
    public int CompareTo(Exam? other)
    {
        if (other is null) return 1;

        int timeComparison = Time.CompareTo(other.Time);
        if (timeComparison != 0) return timeComparison;

        return NumberOfQuestions.CompareTo(other.NumberOfQuestions);
    }

    /// <summary>Create a shallow copy of the exam</summary>
    public virtual object Clone()
    {
        var cloned = new Question[Questions.Length];
        Array.Copy(Questions, cloned, Questions.Length);
        return CreateClone(cloned);
    }

    /// <summary>Override to create specific exam type during cloning</summary>
    protected abstract Exam CreateClone(Question[] clonedQuestions);

    public override string ToString()
    {
        return $"Exam - Time: {Time} min, Questions: {NumberOfQuestions}, Subject: {Subject.Name}, Mode: {Mode}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Exam other) return false;

        return Time == other.Time
               && NumberOfQuestions == other.NumberOfQuestions
               && string.Equals(Subject.Name, other.Subject.Name, StringComparison.Ordinal)
               && Mode == other.Mode;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Time, NumberOfQuestions, Subject.Name, Mode);
    }

    /// <summary>Raise exam started event and notify all students</summary>
    protected virtual void OnExamStarted()
    {
        ExamStarted?.Invoke(this, new ExamEventArgs(Subject, this));
        NotifyAllStudents();
    }

    private void NotifyAllStudents()
    {
        Student[] students = Subject.GetEnrolledStudents();
        for (int i = 0; i < students.Length; i++)
        {
            students[i].OnExamStarted(this, new ExamEventArgs(Subject, this));
        }
    }
}
