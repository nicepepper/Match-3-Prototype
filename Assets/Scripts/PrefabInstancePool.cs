using System.Collections.Generic;
using UnityEngine;

public struct PrefabInstancePool<T> where T : MonoBehaviour
{
    private Stack<T> _pool;

    public T GetInstance (T prefab)
    {
        if (_pool == null)
        {
            _pool = new Stack<T>();
        }
        
#if UNITY_EDITOR
        else if (_pool.TryPeek(out T i) && !i)
        {
            _pool.Clear();
        }
#endif

        if (_pool.TryPop(out T instance))
        {
            instance.gameObject.SetActive(true);
        }
        else
        {
            instance = Object.Instantiate(prefab);
        }
        return instance;
    }

    public void Recycle (T instance)
    {
#if UNITY_EDITOR
        if (_pool == null)
        {
            Object.Destroy(instance.gameObject);
            return;
        }
#endif
        
        _pool.Push(instance);
        instance.gameObject.SetActive(false);
    }
}