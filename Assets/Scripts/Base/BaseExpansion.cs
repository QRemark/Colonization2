using UnityEngine;

public class BaseExpansion : MonoBehaviour
{
    [SerializeField] private int _resourcesToExpand = 5;
    [SerializeField] private GameObject _flagPrefab;

    private Vector3? _flagPosition;
    private bool _expanding = false;

    private Base _base;
    private ResourceCounter _counter;
    private GlobalUnitHandler _unitHandler;
    private BaseManager _baseManager;

    private int _lastCount = 0;
    private GameObject _flagInstance;

    public void Init(Base baseRef, ResourceCounter counter, GlobalUnitHandler unitHandler, BaseManager baseManager)
    {
        _base = baseRef;
        _counter = counter;
        _unitHandler = unitHandler;
        _baseManager = baseManager;
    }

    public void SetFlag(Vector3 position)
    {
        _flagPosition = position;
        _expanding = true;

        if (_flagInstance != null)
            Destroy(_flagInstance);

        _flagInstance = Instantiate(_flagPrefab, position, Quaternion.identity);
    }

    private void Update()
    {
        if (_expanding == false || _flagPosition == null)
            return;

        if (_counter.Count - _lastCount >= _resourcesToExpand)
        {
            Unit builder = _unitHandler.FindFreeUnitFromBase(_base);

            if (builder != null)
            {
                builder.MoveTo(_flagPosition.Value);
                builder.OnArrived += BuildNewBase;
                _lastCount += _resourcesToExpand;
                _expanding = false;
            }
        }
    }

    private void BuildNewBase(Unit unit)
    {
        _baseManager.CreateNewBase(_flagPosition.Value, unit);
        unit.OnArrived -= BuildNewBase;

        Destroy(_flagInstance);
        _flagInstance = null;
        _flagPosition = null;
    }
}
