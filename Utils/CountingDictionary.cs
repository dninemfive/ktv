using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices;

namespace d9.ktv;
[ComDefaultInterface(typeof(IEnumerable))]
public class CountingDictionary<K, V> : IEnumerable<KeyValuePair<K,V>>
    where K : notnull
    where V : INumberBase<V>
{
    private readonly Dictionary<K, V> _dict = new();
    public CountingDictionary() { }
    public void Add(K key)
    {
#pragma warning disable CA1854
        // double lookup is unavoidable because i need ref access to the variable
        if (_dict.ContainsKey(key))
#pragma warning restore CA1854
        {
            _dict[key]++;
        }
        else
        {
            _dict[key] = V.One;
        }
    }
    public V this[K key] => _dict.TryGetValue(key, out V? value) ? value : V.Zero;
    IEnumerator<KeyValuePair<K,V>> IEnumerable<KeyValuePair<K,V>>.GetEnumerator() => _dict.GetEnumerator();
    public IEnumerator GetEnumerator() => _dict.GetEnumerator();
    public IEnumerable<KeyValuePair<K, V>> Descending() => _dict.OrderByDescending(x => x.Value);
    public IEnumerable<KeyValuePair<K, V>> Ascending() => _dict.OrderBy(x => x.Value);
}
