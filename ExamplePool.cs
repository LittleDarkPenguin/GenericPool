public enum AttackType
{
    melee,
    distance
}

public class Enemy : MonoBehaviour
{
	[SerializeField] protected int health;
    public int Health => health;
    [SerializeField] protected AttackType attackType;
    public AttackType AttackType => attackType;
}

public class Mage : Enemy
{	
}

public class Warrior : Enemy
{	
}

public class Spider : Enemy
{	
}

public class ExamplePool : ObjectPool<Enemy>
{
	private void Start()
    {
        Vector2 pos = Vector2.zero;
        
        if (TryGetRandomObject(out Enemy randomEnemy))
        {
            randomEnemy.transform.position = pos;
            pos.x++;
        }
        
        if (TryGetFirstSpecificObject(x => x.Health > 10, out Enemy specificEnemy))
        {
            specificEnemy.transform.position = pos;
            pos.x++;
        }

        if (TryGetFirstSpecificObject(x => x.GetType() == typeof(Warrior), out Enemy warriorEnemy))
        {
            warriorEnemy.transform.position = pos;
            pos.x++;
        }
        
        if (TryGetSpecificObjects(x => x.AttackType == AttackType.melee,
            10, out List<Enemy> specificEnemies, false))
        {
            foreach (var enemy in specificEnemies)
            {
                enemy.transform.position = pos;
                pos.x += 1;
            }
        }
	}
}
