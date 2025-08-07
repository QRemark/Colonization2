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
    private bool _slotReserved = false;

    private Base _base;
    private ResourceCounter _counter;
    private GlobalUnitHandler _unitHandler;
    private GlobalBaseHandler _baseHandler;

    public bool IsLocked => _isLocked;

    private void Update()
    {
        if (_expanding && _flagPlacer.HasFlag)
        {
            StartExpansion();
        }
    }

    private void OnDestroy()
    {
        if (_counter != null)
            _counter.CountChanged -= OnResourceCountChanged;

        if (_slotReserved)
            _baseHandler.ReleaseReservedSlot();
    }

    public void Init(Base baseRef, ResourceCounter counter, GlobalUnitHandler unitHandler, GlobalBaseHandler baseHandler)
    {
        _base = baseRef;
        _counter = counter;
        _unitHandler = unitHandler;
        _baseHandler = baseHandler;

        _counter.CountChanged += OnResourceCountChanged;
    }

    public void SetFlag(Vector3 position)
    {
        if (_isLocked)
            return;

        if (_slotReserved)
        {
            _flagPlacer.Place(position);
            return;
        }

        if (_baseHandler.TryReserveSlot() == false)
            return;

        _flagPlacer.Place(position);
        _expanding = true;
        _waitingForBuilder = false;
        _slotReserved = true;

        _base.SetExpansionMode(true);
    }

    public void OnUnitIdle(Unit unit)
    {
        if (_waitingForBuilder && _expanding && _flagPlacer.HasFlag)
        {
            StartExpansion();
        }
    }

    public void OnResourceCountChanged(int newCount)
    {
        if (_waitingForBuilder && _expanding && _flagPlacer.HasFlag)
        {
            StartExpansion();
        }
    }

    private void StartExpansion()
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
            .FirstOrDefault(unit =>unit.GetAssignedBase() == _base && unit.ReadyForNextTask);
    }

    private void StartExpansion(Unit builder)
    {
        Vector3 targetPos = _flagPlacer.FlagPosition.Value;

        builder.StartBaseBuildingTask(targetPos);
        builder.OnArrived += Build;

        _isLocked = true;
        _expanding = false;
        _waitingForBuilder = false;

        _base.NotifyBuilderSent();
    }

    private void Build(Unit unit)
    {
        if (_flagPlacer.HasFlag == false)
            return;

        Vector3 targetPos = _flagPlacer.FlagPosition.Value;
        float distance = Vector3.Distance(unit.transform.position, targetPos);

        if (distance > _minDistanceToBuild)
        {
            _baseHandler.ReleaseReservedSlot();
            return;
        }

        _baseHandler.Create(targetPos, unit);
        unit.OnArrived -= Build;

        _flagPlacer.Clear();
        _expanding = false;
        _isLocked = false;
        _slotReserved = false;

        _base.SetExpansionMode(false);
    }
}