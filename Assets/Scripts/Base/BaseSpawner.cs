using UnityEngine;

public class BaseSpawner : Spawner<Base>
{
    [SerializeField] private float _spawnY = 12f;

    public Base SpawnBase(Vector3 position)
    {
        Vector3 spawnPos = new Vector3(position.x, _spawnY, position.z);
        Base newBase = SpawnObject(spawnPos, Quaternion.identity);
        return newBase;
    }


    public override void ReturnToPool(Base obj)
    {
        base.ReturnToPool(obj);
    }
}