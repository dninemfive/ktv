using d9.utl;
using d9.utl.compat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
internal class Tree<K>
    where K : notnull
{
    private readonly Dictionary<K, Tree<K>> _dict = new();
    public K Key { get; private set; }
    public Tree<K>? Parent { get; private set; }
    public Tree<K> this[K key]
    {
        get => _dict[key];
        set => _dict[key] = value;
    }
    public Tree<K>? this[params K[] path]
    {
        get
        {
            if (path.Length < 1)
                throw new ArgumentException("Path to a tree must be of length one or greater.", nameof(path));
            Tree<K> cur = this;
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
    private Tree(K key, Tree<K>? parent = null)
    {
        Key = key;
        Parent = parent;
    }
    public string Path 
    {
        get
        {
            List<K> result = new();
            Tree<K>? cur = this;
            while (cur is not null)
            {
                result.Insert(0, cur.Key);
                cur = cur.Parent;
            }
            return result.Select(x => x.PrintNull()).Aggregate((x, y) => $"{x}/{y}");
        }
    }
    public void Add(params K[] kk)
    {
        Tree<K> cur = this;
        foreach(K k in kk)
        {
            cur[k] = new(k);
            cur = cur[k];
        }
    }
}
/*
public class ActivityTreeThing
{
    string Node;
    int Count;
    GoogleUtils.EventColor Color;
    public static Tree<ActivityTreeThing> Tree = new();
}
*/