using System.Linq;
using UnityEngine;

public class BaseExpansion : MonoBehaviour
{
    [SerializeField] private int _resourcesToExpand = 5;
    [SerializeField] private float _minDistanceToBuild = 7f;
    [SerializeField] private FlagPlacer _flagPlacer;

    private bool _expanding = false;
    private bool _waitingForBuilder = false;
    private bool _isLocked = false;

    private Base _base;
    private ResourceCounter _counter;
    private GlobalUnitHandler _unitHandler;
    private BaseManager _baseManager;

    public bool IsLocked => _isLocked;

    private void Update()
    {
        if (_expanding && _flagPlacer.HasFlag)
        {
            TryStartExpansion();
        }
    }

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
            return;

        _flagPlacer.PlaceFlag(position);
        _expanding = true;
        _waitingForBuilder = false;

        _base.SetExpansionMode(true);
    }

    public void OnUnitIdleFromThisBase(Unit unit)
    {
        if (_waitingForBuilder && _expanding && _flagPlacer.HasFlag)
        {
            TryStartExpansion();
        }
    }

    public void OnResourceCountChanged(int newCount)
    {
        if (_waitingForBuilder && _expanding && _flagPlacer.HasFlag)
        {
            TryStartExpansion();
        }
    }

    private void TryStartExpansion()
    {
        if (HasEnoughResources() == false)
        {
            _waitingForBuilder = true;
            return;
        }

        Unit builder = FindIdleBuilder();

        if (builder == null)
        {
            _waitingForBuilder = true;
            return;
        }

        if (_counter.Decrement(_resourcesToExpand) == false)
            return;

        StartExpansion(builder);
    }

    private bool HasEnoughResources()
    {
        return _counter.Count >= _resourcesToExpand;
    }

    private Unit FindIdleBuilder()
    {
        return _unitHandler.GetAll()
            .FirstOrDefault(unit =>
                unit.GetAssignedBase() == _base &&
                unit.ReadyForNewTask);
    }

    private void StartExpansion(Unit builder)
    {
        Vector3 targetPos = _flagPlacer.FlagPosition.Value;

        builder.StartBaseBuildingTask(targetPos);
        builder.OnArrived += BuildNewBase;

        _isLocked = true;
        _expanding = false;
        _waitingForBuilder = false;

        _base.NotifyBuilderSent();
    }

    private void BuildNewBase(Unit unit)
    {
        if (_flagPlacer.HasFlag == false)
            return;

        Vector3 targetPos = _flagPlacer.FlagPosition.Value;
        float distance = Vector3.Distance(unit.transform.position, targetPos);

        if (distance > _minDistanceToBuild)
            return;

        _baseManager.CreateNew(targetPos, unit);
        unit.OnArrived -= BuildNewBase;

        _flagPlacer.ClearFlag();
        _expanding = false;
        _isLocked = false;

        _base.SetExpansionMode(false);
    }
}
