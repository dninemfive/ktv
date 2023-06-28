using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
internal class Tree<K, V>
{
    private readonly Dictionary<K, Tree<K, V>> _dict = new();
    public K Key { get; private set; }
    public V? Value { get; private set; }
    public Tree<K, V>? Parent { get; private set; }
    public Tree<K, V> this[K key] => _dict[key];
    public bool HasValue => Value is not null;
    public bool IsLeaf => HasValue && _dict.Count == 0;
    public bool IsRoot => Parent is not null;
    public Tree(K key, Tree<K, V>? parent = null, V? value = default)
    {
        Key = key;
        Parent = parent;
        Value = value;
    }
    public Tree(K key, V value) : this(key, null, value) { }
}
