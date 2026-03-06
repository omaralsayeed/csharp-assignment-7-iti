namespace D7Task.Core;

/// <summary>Subject with enrolled students for exam management</summary>
public sealed class Subject
{
    public string Name { get; }
    private Student[] _enrolledStudents;
    private int _enrollmentCount;

    public int EnrollmentCount => _enrollmentCount;

    public Subject(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Subject name required.", nameof(name));

        Name = name;
        _enrolledStudents = new Student[4];
    }

    /// <summary>Register a student for this subject</summary>
    public void Enroll(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        EnsureCapacity(_enrollmentCount + 1);
        _enrolledStudents[_enrollmentCount++] = student;
    }

    /// <summary>Notify all enrolled students</summary>
    public void NotifyStudents()
    {
        for (int i = 0; i < _enrollmentCount; i++)
        {
            Console.WriteLine($"[NOTIFY] {_enrolledStudents[i].Name}: Enrolled in '{Name}'");
        }
    }

    /// <summary>Get copy of all enrolled students</summary>
    public Student[] GetEnrolledStudents()
    {
        var result = new Student[_enrollmentCount];
        Array.Copy(_enrolledStudents, result, _enrollmentCount);
        return result;
    }

    private void EnsureCapacity(int required)
    {
        if (_enrolledStudents.Length >= required) return;

        int newCapacity = _enrolledStudents.Length * 2;
        if (newCapacity < required) newCapacity = required;

        Array.Resize(ref _enrolledStudents, newCapacity);
    }
}
