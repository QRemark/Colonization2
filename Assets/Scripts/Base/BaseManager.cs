using UnityEngine;

public class BaseManager : MonoBehaviour
{
    [SerializeField] private GameObject _basePrefab;
    [SerializeField] private Transform _initialBasePosition;

    [SerializeField] private GlobalUnitHandler _globalUnitHandler;
    [SerializeField] private ResourceStorage _resourceStorage;
    [SerializeField] private int _initialUnitCount = 3;

    private void Start()
    {
        CreateInitialBase(_initialBasePosition.position);
    }

    private void CreateInitialBase(Vector3 position)
    {
        GameObject baseGO = Instantiate(_basePrefab, position, Quaternion.identity);
        Base baseComponent = baseGO.GetComponent<Base>();
        baseComponent.Init(
            _globalUnitHandler.GetComponent<UnitSpawner>(),
            _resourceStorage,
            _globalUnitHandler,
            this
        );

        for (int i = 0; i < _initialUnitCount; i++)
        {
            _globalUnitHandler.SpawnUnitForBase(position, baseComponent);
        }
    }

    public void CreateNewBase(Vector3 position, Unit builder)
    {
        GameObject baseGO = Instantiate(_basePrefab, position, Quaternion.identity);
        Base newBase = baseGO.GetComponent<Base>();
        newBase.Init(
            _globalUnitHandler.GetComponent<UnitSpawner>(),
            _resourceStorage,
            _globalUnitHandler,
            this
        );

        builder.AssignToBase(newBase);
        builder.Initialize(position);
    }
}
