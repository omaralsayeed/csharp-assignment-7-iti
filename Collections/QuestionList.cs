using D7Task.Abstractions;
using D7Task.Questions;

namespace D7Task.Collections;

/// <summary>Array-backed list that logs questions to file on add</summary>
public sealed class QuestionList
{
    private readonly string _logFilePath;
    private Question[] _items;
    private int _count;

    public int Count => _count;

    public Question this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();
            return _items[index];
        }
    }

    public QuestionList(string logFilePath, int initialCapacity = 4)
    {
        if (string.IsNullOrWhiteSpace(logFilePath))
            throw new ArgumentException("File path required.", nameof(logFilePath));

        _logFilePath = logFilePath;
        _items = new Question[initialCapacity <= 0 ? 4 : initialCapacity];
    }

    /// <summary>Add question and append to log file</summary>
    public void Add(Question question)
    {
        if (question != null)
        {
            EnsureCapacity(_count + 1);
            _items[_count++] = question;

            using var writer = new StreamWriter(_logFilePath, append: true);
            writer.WriteLine($"{DateTime.Now:O} | {question}");
        }
    }

    /// <summary>Read all lines from log file</summary>
    public string[] ReadLogLines()
    {
        if (!File.Exists(_logFilePath))
            return [];

        using var reader = new StreamReader(_logFilePath);
        string[] lines = new string[8];
        int count = 0;

        while (!reader.EndOfStream)
        {
            EnsureLineCapacity(ref lines, count + 1);
            lines[count++] = reader.ReadLine() ?? string.Empty;
        }

        var result = new string[count];
        Array.Copy(lines, result, count);
        return result;
    }

    private void EnsureCapacity(int required)
    {
        if (_items.Length >= required) return;

        int newCapacity = _items.Length * 2;
        if (newCapacity < required) newCapacity = required;

        Array.Resize(ref _items, newCapacity);
    }

    private static void EnsureLineCapacity(ref string[] lines, int required)
    {
        if (lines.Length >= required) return;

        int newCapacity = lines.Length * 2;
        if (newCapacity < required) newCapacity = required;

        Array.Resize(ref lines, newCapacity);
    }
}
