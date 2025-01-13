namespace CoreUtilities.Helpers.Extensions;

using CoreUtilities.Helpers.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Extensions for <see cref="IEnumerable{T}"/> classes.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Clone of a <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The data type the <see cref="IEnumerable{T}"/> stores.</typeparam>
    /// <param name="toCopy">The <see cref="IEnumerable{T}"/> to copy.</param>
    /// <returns>A copy of the given <see cref="IEnumerable{T}"/>.</returns>
    public static IEnumerable<T> ShallowCopy<T>(this IEnumerable<T> toCopy)
    {
        return new List<T>(toCopy);
    }

    public static RangeObservableCollection<T> ToRangeObservableCollection<T>(this IEnumerable<T> enumerable)
    {
        return new RangeObservableCollection<T>(enumerable);
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
    {
        foreach (var item in enumerable)
        {
            if (item != null)
            {
                yield return item!;
            }
        }
    }

    public static IEnumerable<TOut> SelectNotNull<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut?> itemSelector)
    {
        foreach (var item in enumerable)
        {
            var transformedItem = itemSelector(item);
            if (transformedItem != null)
            {
                yield return transformedItem;
            }
        }
    }
}
