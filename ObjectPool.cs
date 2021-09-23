using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class ObjectPool<P> : MonoBehaviour where P : Component
{
    [SerializeField] protected P[] objPrefabs;
    [SerializeField] protected int initialAmount = 10;
    
    private Dictionary<System.Type, Queue<P>> pool = new Dictionary<System.Type, Queue<P>>();
    private List<System.Type> keys;
    
    private void Awake()
    {
        foreach (var enemy in objPrefabs)
        {
            InitialFill(enemy, transform, initialAmount);
        }
        keys = new List<System.Type>(pool.Keys);
    }

    private void InitialFill(P prefab, Transform parent, int amount = 10)
    {
        var t = prefab.GetType();
        if (!pool.ContainsKey(t))
        {
            pool.Add(t, new Queue<P>());
            for (int i = 0; i < amount; i++)
            {
                P obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
                pool[t].Enqueue(obj);
                obj.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError($"Trying to add existing type |{prefab}| to pool, skipping");
        }

    }

    public void ReturnObjectToPool(P component)
    {
        if (pool.TryGetValue(component.GetType(), out Queue<P> queue))
        {
            component.gameObject.SetActive(false);
            queue.Enqueue(component);
        }
        else
        {
            Debug.Log($"Returning object with non existing type in pool |{component}|, adding new queue");
            var newQueue = new Queue<P>();
            pool.Add(component.GetType(), newQueue);
            pool[component.GetType()].Enqueue(component);
        }
    }

    private P TryGetObjectByType(System.Type type)
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
