using UnityEngine;

public class BaseSpawner : Spawner<Base>
{
    [SerializeField] private float _spawnY = 12f;
    private Quaternion _defaultRotation = Quaternion.identity;
    private int _currentBaseCount = 0;
    private int _defoultBaseCount = 0;

    public Base Create(Vector3 position)
    {
        if (_currentBaseCount >= _initialSize)
        {
            return null;
        }

        Vector3 spawnPos = new Vector3(position.x, _spawnY, position.z);
        Base newBase = SpawnObject(spawnPos, _defaultRotation);

        if (newBase != null)
            _currentBaseCount++;

        return newBase;
    }

    public override void ReturnToPool(Base obj)
    {
        base.ReturnToPool(obj);
        _currentBaseCount = Mathf.Max(_defoultBaseCount, _currentBaseCount - 1);
    }

    public int GetMaxBaseCount()
    {
        return _initialSize;
    }
}
