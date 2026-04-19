using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Skyjo.Network.Utils;

public sealed class IndexedCollection<TKey, T> : IEnumerable<T>
    where TKey : notnull
{
    private readonly List<T> _items = [];
    private readonly Dictionary<TKey, int> _indices = [];
    private readonly Func<T, TKey> _keySelector;

    public IndexedCollection(Func<T, TKey> keySelector)
    {
        _keySelector = keySelector;
    }

    public int Count => _items.Count;

    public void Add(T item)
    {
        var key = _keySelector(item);
        _indices[key] = _items.Count;
        _items.Add(item);
    }

    public bool Contains(TKey key) => _indices.ContainsKey(key);

    public void Clear()
    {
        _items.Clear();
        _indices.Clear();
    }

    public T this[TKey key]
    {
        get => _items[_indices[key]];
        set
        {
            if (!EqualityComparer<TKey>.Default.Equals(_keySelector(value), key))
                throw new ArgumentException("The key extracted from the value does not match the indexer key.");
            _items[_indices[key]] = value;
        }
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
    {
        if (_indices.TryGetValue(key, out var index))
        {
            value = _items[index];
            return true;
        }

        value = default;
        return false;
    }

    public T? GetValueOrDefault(TKey key)
    {
        return TryGetValue(key, out var value) ? value : default;
    }

    public bool Remove(TKey key)
    {
        if (!_indices.TryGetValue(key, out var index))
            return false;

        var lastIndex = _items.Count - 1;

        if (index != lastIndex)
        {
            var lastItem = _items[lastIndex];
            _items[index] = lastItem;
            _indices[_keySelector(lastItem)] = index;
        }

        _items.RemoveAt(lastIndex);
        _indices.Remove(key);
        return true;
    }

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
}