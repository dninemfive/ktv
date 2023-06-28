using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
internal class Tree<K, V>
    where K : notnull
{
    private readonly Dictionary<K, Tree<K, V>> _dict = new();
    public K Key { get; private set; }
    public V? Value { get; private set; }
    public Tree<K, V>? Parent { get; private set; }
    public Tree<K, V> this[K key] => _dict[key];
    public Tree<K, V>? this[params K[] path]
    {
        get
        {
            if (path.Length < 1)
                throw new ArgumentException("Path to a tree must be of length one or greater.", nameof(path));
            Tree<K, V> cur = this;
            foreach(K key in path)
            {
                try
                {
                    cur = cur[key];
                } catch(Exception ex)
                {
                    Utils.DebugLog(ex);
                    return null;
                }
            }
            return cur;
        }
    }
    public bool HasValue => Value is not null;
    public bool IsLeaf => HasValue && _dict.Count == 0;
    public bool IsRoot => Parent is not null;
    private Tree(K key, Tree<K, V>? parent = null)
    {
        Key = key;
        Parent = parent;
    }
    public Tree(K key, (K key, V value) child, Tree<K, V>? parent = null) : this(key, parent)
    {
        Add(child);
    }
    public Tree(K key, V value, Tree<K, V>? parent = null) : this(key, parent)
    {
        Value = value;
    }
    public void Add(K key, V value) => _dict[key] = new(key, value, this);
    public void Add((K key, V value) tuple) => Add(tuple.key, tuple.value);
    public string Path 
    {
        get
        {
            List<K> result = new();
            Tree<K, V>? cur = this;
            while (cur is not null)
            {
                result.Insert(0, cur.Key);
                cur = cur.Parent;
            }
            return result.Select(x => x.PrintNull()).Aggregate((x, y) => $"{x}/{y}");
        }
    }
}
