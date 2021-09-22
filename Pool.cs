using System.Collections.Generic;
using UnityEngine;

public class Pool : ObjectPool<EnemyType, Enemy>
{
    [SerializeField] private Enemy[] enemiesPrefabs;
    [SerializeField] private int initialAmount = 10;
    
    private void Awake()
    {
        foreach (var enemy in enemiesPrefabs)
        {
            InitialFill(enemy.Type, enemy, transform, initialAmount);
        }
        keys = new List<EnemyType>(pool.Keys);
    }

    public void ReturnObjectToPool(Enemy enemy)
    {
        base.ReturnObjectToPool(enemy, enemy.Type);
    }
    
    // ?? well mb it will be usefull somehow
    public Enemy TryGetEnemyByComponent(Enemy enemy)
    {
        return TryGetObjectByType(enemy.Type);
    }
}
