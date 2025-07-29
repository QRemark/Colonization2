using UnityEngine;

public class BaseSpawner : Spawner<Base>
{
    public Base SpawnBase(Vector3 position)
    {
        Base newBase = SpawnObject(position, Quaternion.identity);
        return newBase;
    }

    public override void ReturnToPool(Base obj)
    {
        base.ReturnToPool(obj);
    }
}
