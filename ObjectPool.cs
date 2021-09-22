using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T, P> : MonoBehaviour where P : Enemy
{
    protected Dictionary<T, Queue<P>> pool = new Dictionary<T, Queue<P>>();
    protected List<T> keys;

    protected void InitialFill(T type, P prefab, Transform parent, int amount = 10)
    {
        if (!pool.ContainsKey(type))
        {
            pool.Add(type, new Queue<P>());
            for (int i = 0; i < amount; i++)
            {
                P obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
                pool[type].Enqueue(obj);
                obj.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError($"Trying to add existing type |{type}| to pool, skipping");
        }
    }

    protected void ReturnObjectToPool(P component, T type)
    {
        if (pool.TryGetValue(type, out Queue<P> queue))
        {
            component.gameObject.SetActive(false);
            queue.Enqueue(component);
        }
        else
        {
            Debug.Log($"Returning object with non existing type in pool |{type}|, adding new queue");
            pool.Add(type, new Queue<P>());
            pool[type].Enqueue(component);
        }
    }

    public P TryGetObjectByType(T type)
    {
        if (pool.TryGetValue(type, out Queue<P> queue))
        {
            if (queue.Count > 1)
            {
                P obj = queue.Dequeue();
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                Debug.Log($"Queue for |{type}| almost empty, creating new obj for it");
                P obj = Instantiate(queue.Peek(), Vector3.zero, Quaternion.identity);
                return obj;
            }
        }
        Debug.LogError($"Asking for non existing type in pool |{type}|, returning null");
        return null;
    }
    
    public P GetRandomObject()
    {
        int randomIndex = Random.Range(0, keys.Count);
        return TryGetObjectByType(keys[randomIndex]);
    }
}
