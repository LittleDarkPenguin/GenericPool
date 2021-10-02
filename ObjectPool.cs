using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectPool<P> : MonoBehaviour where P : MonoBehaviour
{
    [SerializeField] private P[] prefabs;
    [SerializeField] private int initialAmount = 10;

    private Dictionary<P, Queue<P>> pool;
    private List<P> keys;

    private void Awake()
    {
        InitialFill();
    }

    private void InitialFill()
    {
        pool = new Dictionary<P, Queue<P>>();
        foreach (var pref in prefabs)
        {
            if (!pool.ContainsKey(pref))
            {
                pool.Add(pref, new Queue<P>());
                
                for (int i = 0; i < initialAmount; i++)
                {
                    var obj = Instantiate(pref, Vector3.zero, Quaternion.identity, transform);
                    obj.gameObject.SetActive(false);
                    pool[pref].Enqueue(obj);
                }
            }
            else
            {
                Debug.Log($"Trying to add existing prefab \"{pref}\" to pool, skipping");
            }
        }
        keys = new List<P>(pool.Keys);
    }

    private bool TryGetQueueForType(P type, out Queue<P> queue)
    {
        queue = null;
        foreach (var key in pool.Keys)
        {
            if (key.GetType() == type.GetType())
            {
                queue = pool[key];
                return true;
            }
        }
        return false;
    }
    
    private bool TryGetObjectByType(P type, out P obj)
    {
        obj = null;
        if (TryGetQueueForType(type, out var queue))
        {
            if (queue.Count > 1)
            {
                obj = queue.Dequeue();
            }
            else
            {
                Debug.Log($"Queue for |{type}| almost empty, creating new obj for it");
                obj = Instantiate(queue.Peek(), Vector3.zero, Quaternion.identity, transform);
            }
            return true;
        }
        Debug.LogError($"Asking for non existing object in pool \"{type}\" (what is nearly impossible btw), returning null");
        return false;
    }
    
    public bool TryGetRandomObject(out P obj)
    {
        obj = null;
        if (pool.Count == 0)
        {
            Debug.LogError("Pool is empty, returning null");
            return false;
        }
        int randomIndex = Random.Range(0, keys.Count);
        if (TryGetObjectByType(keys[randomIndex], out P randObj))
        {
            obj = randObj;
            obj.gameObject.SetActive(true);
            return true;
        }
        return false;
    }
    
    public void ReturnObjectToPool(P obj)
    {
        if (TryGetQueueForType(obj, out var queue))
        {
            queue.Enqueue(obj);
            obj.gameObject.SetActive(false);
            return;
        }

        Debug.Log($"Returning object that doesn't exist in pool \"{obj}\", adding new queue");
        var newQueue = new Queue<P>();
        pool.Add(obj, newQueue);
        pool[obj].Enqueue(obj);
        obj.gameObject.SetActive(false);
    }

    public bool TryGetFirstSpecificObject(Predicate<P> propertySelector, out P obj)
    {
        obj = null;
        var type = keys.Find(propertySelector);
        if (type == null)
        {
            Debug.Log("Nothing was found by your request, returning null");
            return false;
        }

        if (TryGetObjectByType(type, out P newObj))
        {
            obj = newObj;
            obj.gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    public bool TryGetSpecificObjects(Predicate<P> propertySelector, int amount, out List<P> objects, bool randomize = true)
    {
        objects = null;
        amount = Math.Abs(amount);
        // Really dont like this
        var types = keys.FindAll(propertySelector);
        if (types.Count == 0)
        {
            Debug.Log("Nothing was found by your request");
            return false;
        }

        objects = new List<P>(amount);
        if (randomize)
        {
            for (int i = 0; i < amount; i++)
            {
                var randomType = types[Random.Range(0, types.Count)];
                if (TryGetObjectByType(randomType, out P obj))
                {
                    objects.Add(obj);
                }
                else
                {
                    Debug.Log($"Something was broking while trying to get object by type \"{randomType}\"");
                }
            }
        }
        else
        {
            int amountOfType = amount / types.Count;
            foreach (var type in types)
            {
                for (int i = 0; i < amountOfType; i++)
                {
                    if (TryGetObjectByType(type, out P obj))
                    {
                        objects.Add(obj);
                    }
                    else
                    {
                        Debug.Log($"Something was broking while trying to get object by type \"{type}\"");
                    }
                }
            }
            if (amount % types.Count != 0)
            {
                var randomType = types[Random.Range(0, types.Count)];
                if (TryGetObjectByType(randomType, out P obj))
                {
                    objects.Add(obj);
                }
                else
                {
                    Debug.Log($"Something was broking while trying to get object by type \"{randomType}\"");
                }
            }
        }
        return true;
    }
}
