namespace D7Task.Models;

/// <summary>Answer option for a question</summary>
public class Answer : IComparable<Answer>, ICloneable
{
    public int Id { get; }
    public string Text { get; }

    public Answer(int id, string text)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text required.", nameof(text));

        Id = id;
        Text = text;
    }

    /// <summary>Compare answers by id</summary>
    public int CompareTo(Answer? other)
    {
        if (other is null) return 1;
        return Id.CompareTo(other.Id);
    }

    public virtual object Clone() => new Answer(Id, Text);

    public override string ToString() => $"{Id}. {Text}";

    public override bool Equals(object? obj)
    {
        if (obj is not Answer other) return false;
        return Id == other.Id && string.Equals(Text, other.Text, StringComparison.Ordinal);
    }

    public override int GetHashCode() => HashCode.Combine(Id, Text);
}
