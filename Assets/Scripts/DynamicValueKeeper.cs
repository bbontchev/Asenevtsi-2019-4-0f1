using System;
using System.Collections.Concurrent;
using UnityEngine;

//d-carev added a threadsafe singleton dynamic value storage class implemented as DynamicValueKeeper.
//It can be used for the much needed dynamic settings storage.

public sealed class DynamicValueKeeper : MonoBehaviour
{
    private static readonly object locker = new object();

    private static DynamicValueKeeper valueKeeperInstance = null;

    private static ConcurrentDictionary<string, object> valueDictionary = null;

    public static DynamicValueKeeper Instance
    {
        get
        {
            lock (locker)
            {
                if (valueDictionary == null)
                    valueKeeperInstance = new DynamicValueKeeper();
            }

            return valueKeeperInstance;
        }
    }

    private DynamicValueKeeper()
    {
        valueDictionary = new ConcurrentDictionary<string, object>();
    }

    private void Awake()
    {
        lock (locker)
        {
            if (valueKeeperInstance != null && valueKeeperInstance != this)
                Destroy(gameObject);
            else
                valueKeeperInstance = this;
        }
    }

    public bool SaveValue<T>(string Key, T Value)
    {
        return valueDictionary.TryAdd(Key, Value);
    }

    public bool RemoveValue(string Key)
    {
        object value;
        return valueDictionary.TryRemove(Key, out value);
    }

    public Type GetValueType(string Key)
    {
        if (string.IsNullOrEmpty(Key))
            return null;

        object value = null;
        if (!valueDictionary.TryGetValue(Key, out value))
            return null;

        if (value == null)
            return null;

        return value.GetType();
    }

    public bool GetValue(string Key, out object Value)
    {
        return valueDictionary.TryGetValue(Key, out Value);
    }

    public bool ValueExists(string Key)
    {
        if (string.IsNullOrEmpty(Key))
            return false;

        return valueDictionary.ContainsKey(Key);
    }
}