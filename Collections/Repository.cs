namespace D7Task.Collections;

/// <summary>Generic repository with sort and clone capabilities</summary>
public sealed class Repository<T> where T : ICloneable, IComparable<T>
{
    private T[] _items;
    private int _count;

    public int Count => _count;

    public Repository(int initialCapacity = 4)
    {
        if (initialCapacity <= 0) initialCapacity = 4;
        _items = new T[initialCapacity];
    }

    /// <summary>Add item to repository</summary>
    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureCapacity(_count + 1);
        _items[_count++] = item;
    }

    /// <summary>Remove item from repository</summary>
    public void Remove(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        int index = -1;
        for (int i = 0; i < _count; i++)
        {
            if (_items[i]!.Equals(item))
            {
                index = i;
                break;
            }
        }

        if (index == -1)
            throw new InvalidOperationException("Item not found.");

        for (int i = index; i < _count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }

        _count--;
    }

    /// <summary>Sort all items using QuickSort</summary>
    public void Sort()
    {
        if (_count <= 1) return;
        QuickSort(0, _count - 1);
    }

    /// <summary>Get copy of all items</summary>
    public T[] GetAll()
    {
        var result = new T[_count];
        Array.Copy(_items, result, _count);
        return result;
    }

    private void QuickSort(int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(low, high);
            QuickSort(low, pi - 1);
            QuickSort(pi + 1, high);
        }
    }

    private int Partition(int low, int high)
    {
        T pivot = _items[high]!;
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (_items[j]!.CompareTo(pivot) < 0)
            {
                i++;
                (_items[i], _items[j]) = (_items[j], _items[i]);
            }
        }

        (_items[i + 1], _items[high]) = (_items[high], _items[i + 1]);
        return i + 1;
    }

    private void EnsureCapacity(int required)
    {
        if (_items.Length >= required) return;

        int newCapacity = _items.Length * 2;
        if (newCapacity < required) newCapacity = required;

        Array.Resize(ref _items, newCapacity);
    }
}
