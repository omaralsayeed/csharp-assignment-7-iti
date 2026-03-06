using D7Task.Abstractions;

namespace D7Task.Core;

/// <summary>Holds event data when exam starts</summary>
public class ExamEventArgs : EventArgs
{
    public Subject Subject { get; }
    public Exam Exam { get; }

    public ExamEventArgs(Subject subject, Exam exam)
    {
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        Exam = exam ?? throw new ArgumentNullException(nameof(exam));
    }
}
