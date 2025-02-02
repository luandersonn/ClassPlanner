using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClassPlanner.Collections;

public class ObservableSortedCollection<T>(IComparer<T> comparer) : ObservableCollection<T>, IObservableSortedCollection<T>
{
    public IComparer<T> Comparer { get; } = comparer ?? throw new ArgumentNullException(nameof(comparer));

    public ObservableSortedCollection(IComparer<T> comparer, IEnumerable<T> collection) : this(comparer)
    {
        ArgumentNullException.ThrowIfNull(collection);

        foreach (var item in collection)
        {
            AddItem(item);
        }
    }

    public void AddItem(T item)
    {
        var index = BinarySearch(item);
        if (index < 0)
            index = ~index;
        Insert(index, item);
    }

    public bool RemoveItem(T item)
    {
        var index = BinarySearch(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    public int BinarySearch(T item)
    {
        var min = 0;
        var max = Count - 1;
        while (min <= max)
        {
            var mid = (min + max) / 2;
            if (Comparer.Compare(item, this[mid]) == 0)
                return mid;
            else if (Comparer.Compare(item, this[mid]) < 0)
            {
                max = mid - 1;
            }
            else
            {
                min = mid + 1;
            }
        }
        return ~(max + 1);
    }
}
