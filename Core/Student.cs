using D7Task.Core;

namespace D7Task.Core;

/// <summary>Represents a student in the system</summary>
public sealed class Student
{
    public int Id { get; }
    public string Name { get; }

    public Student(int id, string name)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name required.", nameof(name));

        Id = id;
        Name = name;
    }

    /// <summary>Handle when an exam starts (event subscription)</summary>
    public void OnExamStarted(object sender, ExamEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        Console.WriteLine($"[EVENT] {Name} received: {e.Subject.Name} exam started.");
    }

    public override string ToString() => $"{Id} - {Name}";
}
