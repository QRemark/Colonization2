using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseExpansion : MonoBehaviour
{
    [SerializeField] private int _resourcesToExpand = 5;
    [SerializeField] private GameObject _flagPrefab;

    private Vector3? _flagPosition;
    private bool _expanding = false;
    private bool _waitingForBuilder = false;

    private Base _base;
    private ResourceCounter _counter;
    private GlobalUnitHandler _unitHandler;
    private BaseManager _baseManager;

    private Quaternion _defaultRotation = Quaternion.identity;
    private GameObject _flagInstance;
    private bool _isLocked = false;

    private float _minDistanceToBuild = 7f;

    public bool IsLocked => _isLocked;

    public void Init(Base baseRef, ResourceCounter counter, GlobalUnitHandler unitHandler, BaseManager baseManager)
    {
        _base = baseRef;
        _counter = counter;
        _unitHandler = unitHandler;
        _baseManager = baseManager;

        _counter.CountChanged += OnResourceCountChanged;
    }

    public void SetFlag(Vector3 position)
    {
        if (_isLocked)
        {
            return;
        }

        if (_flagInstance != null)
        {
            Destroy(_flagInstance);
            _flagInstance = null;
        }

        _flagPosition = position;
        _expanding = true;
        _waitingForBuilder = false;

        _flagInstance = Instantiate(_flagPrefab, position, _defaultRotation);
        _base.SetExpansionMode(true);
    }

    public void OnUnitIdleFromThisBase(Unit unit)
    {
        if (_waitingForBuilder  == false|| _expanding == false|| _flagPosition == null)
        {
            return;
        }

        TryStart();
    }

    private void Update()
    {
        if (_expanding == false|| _flagPosition == null)
            return;

        TryStart();
    }

    public void OnResourceCountChanged(int newCount)
    {
        if (_waitingForBuilder && _flagPosition != null && _expanding)
        {
            TryStart();
        }
    }

    private void TryStart()
    {
        if (_counter.Count < _resourcesToExpand)
        {
            if (_waitingForBuilder == false)
            {
                _waitingForBuilder = true;
            }

            return;
        }

        List<Unit> ownIdleUnits = _unitHandler.GetAll()
            .Where(unit => unit.GetAssignedBase() == _base && unit.ReadyForNewTask)
            .ToList();

        Unit builder = ownIdleUnits.FirstOrDefault();

        if (builder == null)
        {
            _waitingForBuilder = true;
            return;
        }

        if (_counter.Decrement(_resourcesToExpand) == false)
        {
            return;
        }

        builder.StartBaseBuildingTask(_flagPosition.Value);
        builder.OnArrived += BuildNew;

        _isLocked = true;
        _expanding = false;
        _waitingForBuilder = false;

        _base.NotifyBuilderSent();
    }

    private void BuildNew(Unit unit)
    {
        float distance = Vector3.Distance(unit.transform.position, _flagPosition.Value);

        if (distance > _minDistanceToBuild)
        {
            return;
        }

        _baseManager.CreateNew(_flagPosition.Value, unit);
        unit.OnArrived -= BuildNew;

        Destroy(_flagInstance);
        _flagInstance = null;
        _flagPosition = null;
        _expanding = false;
        _isLocked = false;
        _base.SetExpansionMode(false);
    }
}
