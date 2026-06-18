using System;
using System.Collections.Generic;
using UnityEngine;


public interface IHashMap<TKey, TValue>
{
    void Add(TKey key, TValue value);
    void Remove(TKey key);
    TValue Lookup(TKey key);
    void Print();
}

public class HashMapADT<TKey, TValue> : IHashMap<TKey, TValue>
{
    private List<KeyValuePair<TKey, TValue>>[] storage;
    private const int storageLimit = 14;

    public HashMapADT()
    {
        storage = new List<KeyValuePair<TKey, TValue>>[storageLimit];
    }

    private int Hash(string key, int max)
    {
        int hash = 0;

        for (int i = 0; i < key.Length; i++)
        {
            hash += key[i];
        }

        return hash % max;
    }

    public void Add(TKey key, TValue value)
    {
        string keyString = key.ToString();
        int index = Hash(keyString, storageLimit);

        if (storage[index] == null)
        {
            storage[index] = new List<KeyValuePair<TKey, TValue>>
            {
                new KeyValuePair<TKey, TValue>(key, value)
            };
        }
        else
        {
            bool inserted = false;

            for (int i = 0; i < storage[index].Count; i++)
            {
                if (storage[index][i].Key.Equals(key))
                {
                    storage[index][i] = new KeyValuePair<TKey, TValue>(key, value);
                    inserted = true;
                    break;
                }
            }

            if (!inserted)
            {
                storage[index].Add(new KeyValuePair<TKey, TValue>(key, value));
            }
        }
    }

    public void Remove(TKey key)
    {
        string keyString = key.ToString();
        int index = Hash(keyString, storageLimit);

        if (storage[index] == null)
        {
            return;
        }

        if (storage[index].Count == 1 && storage[index][0].Key.Equals(key))
        {
            storage[index] = null;
        }
        else
        {
            for (int i = 0; i < storage[index].Count; i++)
            {
                if (storage[index][i].Key.Equals(key))
                {
                    storage[index].RemoveAt(i);
                    break;
                }
            }
        }
    }

    public TValue Lookup(TKey key)
    {
        string keyString = key.ToString();
        int index = Hash(keyString, storageLimit);

        if (storage[index] == null)
        {
            return default(TValue);
        }

        for (int i = 0; i < storage[index].Count; i++)
        {
            if (storage[index][i].Key.Equals(key))
            {
                return storage[index][i].Value;
            }
        }

        return default(TValue);
    }

    public void Print()
    {
        Debug.Log("HashMap contents:");

        for (int i = 0; i < storage.Length; i++)
        {
            if (storage[i] == null)
            {
                continue;
            }

            for (int j = 0; j < storage[i].Count; j++)
            {
                Debug.Log(storage[i][j].Key + " : " + storage[i][j].Value);
            }
        }
    }
}
