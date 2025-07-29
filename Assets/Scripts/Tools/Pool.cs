using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : MonoBehaviour
{
    private readonly Queue<T> _inactiveObjects = new Queue<T>();
    private readonly List<T> _activeObjects = new List<T>();

    private Transform _parent;

    public void Initialize(T prefab, int initialSize)
    {
        _inactiveObjects.Clear();
        _activeObjects.Clear();

        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(prefab, _parent);
            obj.gameObject.SetActive(false);
            _inactiveObjects.Enqueue(obj);
        }
    }

    public void SetParent(Transform parent)
    {
        _parent = parent;
    }

    public T GetObject()
    {
        if (_inactiveObjects.Count == 0)
            return null;

        T obj = _inactiveObjects.Dequeue();
        obj.gameObject.SetActive(true);
        _activeObjects.Add(obj);

        return obj;
    }

    public void ReleaseObject(T obj)
    {
        if (_inactiveObjects.Contains(obj) == false)
        {
            obj.gameObject.SetActive(false);
            _inactiveObjects.Enqueue(obj);
            _activeObjects.Remove(obj);
        }
    }
}