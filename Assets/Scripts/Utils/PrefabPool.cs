using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class PrefabPool<T> where T : MonoBehaviour
{
    private readonly T _prefab;
    private readonly List<T> _activeInstances;
    private readonly List<T> _inactiveInstances;
    private readonly Transform _inactiveContainer;
    private readonly Action<T> _onBeforeGet;
    private readonly Action<T> _onBeforeReturn;

    public PrefabPool(T prefab, Transform inactiveContainer, Action<T> onBeforeGet = null, Action<T> onBeforeReturn = null)
    {
        _prefab = prefab;
        _activeInstances = new List<T>();
        _inactiveInstances = new List<T>();
        _inactiveContainer = inactiveContainer;
        _onBeforeGet = onBeforeGet;
        _onBeforeReturn = onBeforeReturn;
    }

    public T Get()
    {
        if (_inactiveInstances.Count == 0)
        {
            T newT = Object.Instantiate(_prefab, _inactiveContainer);
            _inactiveInstances.Add(newT);
        }
        
        T instance = _inactiveInstances[0];
        _inactiveInstances.Remove(instance);
        _activeInstances.Add(instance);
        _onBeforeGet?.Invoke(instance);
        return instance;
    }

    public void Return(T instance)
    {
        _onBeforeReturn?.Invoke(instance);
        _activeInstances.Remove(instance);
        _inactiveInstances.Add(instance);
        instance.transform.SetParent(_inactiveContainer);
    }
}