using UnityEngine;
using System.Collections.Generic;

public class UnitSpawner : Spawner<Unit>
{
    [SerializeField] private float _spawnRadius = 5f;
    [SerializeField] private float _angleStepDegrees = 30f;
    [SerializeField] private float _yOffset = 0f;

    private List<Unit> _units;
    
    public IEnumerable<Unit> Units => _units;

    protected override void Awake()
    {
        base.Awake();
        _units = new List<Unit>();
    }

    public Unit CreateSingleUnit(Vector3 center)
    {
        int index = _units.Count;
        float angle = index * _angleStepDegrees * Mathf.Deg2Rad;
        float offsetX = Mathf.Cos(angle) * _spawnRadius;
        float offsetZ = Mathf.Sin(angle) * _spawnRadius;

        Vector3 offset = new Vector3(offsetX, _yOffset, offsetZ);
        Vector3 spawnPosition = center + offset;

        Unit unit = SpawnObject(spawnPosition, Quaternion.identity);
        if (unit != null)
        {
            unit.gameObject.SetActive(true);

            if (unit.TryGetComponent(out Rigidbody rb))
            {
                rb.position = spawnPosition;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                unit.transform.position = spawnPosition;
            }

            unit.Initialize(center);
            _units.Add(unit);
        }

        return unit;
    }

    public override void ReturnToPool(Unit unit)
    {
        base.ReturnToPool(unit);
        _units.Remove(unit);
    }
}
