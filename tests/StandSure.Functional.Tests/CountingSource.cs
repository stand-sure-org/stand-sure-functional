using System.Collections;
using System.Collections.Generic;

namespace StandSure.Functional.Tests;

/// <summary>An enumerable that counts GetEnumerator and MoveNext calls, to prove laziness and single enumeration.</summary>
internal sealed class CountingSource<T>(IEnumerable<T> items) : IEnumerable<T>
{
    public int EnumeratorCount { get; private set; }

    public int MoveNextCount { get; private set; }

    public IEnumerator<T> GetEnumerator()
    {
        this.EnumeratorCount++;

        foreach (var item in items)
        {
            this.MoveNextCount++;
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
