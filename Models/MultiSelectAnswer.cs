namespace D7Task.Models;

/// <summary>Answer for questions allowing multiple selections</summary>
public sealed class MultiSelectAnswer : Answer
{
    public int[] SelectedIds { get; }

    public MultiSelectAnswer(int id, string text, int[] selectedIds)
        : base(id, text)
    {
        if (selectedIds is null || selectedIds.Length == 0)
            throw new ArgumentException("Select at least one answer.", nameof(selectedIds));

        SelectedIds = Normalize(selectedIds);
    }

    private static int[] Normalize(int[] values)
    {
        var copy = new int[values.Length];
        Array.Copy(values, copy, values.Length);
        Array.Sort(copy);

        int uniqueCount = 1;
        for (int i = 1; i < copy.Length; i++)
        {
            if (copy[i] != copy[i - 1])
            {
                copy[uniqueCount++] = copy[i];
            }
        }

        var normalized = new int[uniqueCount];
        Array.Copy(copy, normalized, uniqueCount);
        return normalized;
    }
}
