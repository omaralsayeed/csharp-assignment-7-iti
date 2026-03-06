namespace D7Task.Models;

/// <summary>Array-backed list of answers with auto-resizing</summary>
public sealed class AnswerList
{
    private Answer[] _answers;
    private int _count;

    public int Count => _count;

    public Answer this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();
            return _answers[index];
        }
        set
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();
            _answers[index] = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public AnswerList(int capacity = 4)
    {
        if (capacity <= 0) capacity = 4;
        _answers = new Answer[capacity];
    }

    /// <summary>Add answer and resize if needed</summary>
    public void Add(Answer answer)
    {
        ArgumentNullException.ThrowIfNull(answer);
        EnsureCapacity(_count + 1);
        _answers[_count++] = answer;
    }

    /// <summary>Find answer by id</summary>
    public Answer? GetById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_answers[i].Id == id)
                return _answers[i];
        }
        return null;
    }

    private void EnsureCapacity(int required)
    {
        if (_answers.Length >= required) return;

        int newSize = _answers.Length * 2;
        if (newSize < required) newSize = required;

        Array.Resize(ref _answers, newSize);
    }
}
