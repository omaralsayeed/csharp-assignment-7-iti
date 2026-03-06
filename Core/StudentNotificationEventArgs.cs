namespace D7Task.Core;

public sealed class StudentNotificationEventArgs : EventArgs
{
    public Student Student { get; }
    public string Message { get; }

    public StudentNotificationEventArgs(Student student, string message)
    {
        Student = student ?? throw new ArgumentNullException(nameof(student));
        Message = string.IsNullOrWhiteSpace(message)
            ? throw new ArgumentException("Message is required.", nameof(message))
            : message;
    }
}
