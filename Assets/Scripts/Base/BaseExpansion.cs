using UnityEngine;
using System.Linq;

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

    private GameObject _flagInstance;
    private bool _isLocked = false;
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
            Debug.Log($"[BaseExpansion] Нельзя установить флаг — база заблокирована до окончания расширения.");
            return;
        }

        Debug.Log($"[BaseExpansion] SetFlag called with position {position}");

        if (_flagInstance != null)
        {
            Debug.Log("[BaseExpansion] Старый флаг удалён");
            Destroy(_flagInstance);
            _flagInstance = null;
        }

        _flagPosition = position;
        _expanding = true;
        _waitingForBuilder = false;

        _flagInstance = Instantiate(_flagPrefab, position, Quaternion.identity);
        _base.SetExpansionMode(true);
    }


    public void OnUnitIdleFromThisBase(Unit unit)
    {
        Debug.Log($"[BaseExpansion] DEBUG: _waitingForBuilder={_waitingForBuilder}, _expanding={_expanding}, _flagPosition={_flagPosition}");

        if (_waitingForBuilder  == false|| _expanding == false|| _flagPosition == null)
        {
            Debug.Log($"[BaseExpansion] Пропускаем повторную попытку.");
            return;
        }

        Debug.Log("[BaseExpansion] Повторная попытка TryStartExpansion через BecameIdle");
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
            Debug.Log("[BaseExpansion] Retrying builder assignment on CountChanged...");
            TryStart();
        }
    }

    private void TryStart()
    {
        Debug.Log($"[BaseExpansion] TryStartExpansion для базы {_base.name}: ресурсы={_counter.Count}");

        if (_counter.Count < _resourcesToExpand)
        {
            Debug.Log("[BaseExpansion] Not enough resources yet.");

            if (_waitingForBuilder == false)
            {
                Debug.Log("[BaseExpansion] Not enough resources yet, setting waitingForBuilder = true");
                _waitingForBuilder = true;
            }

            return;
        }

        var ownIdleUnits = _unitHandler.GetAll()
            .Where(u => u.GetAssignedBase() == _base && u.ReadyForNewTask)
            .ToList();

        foreach (var unit in ownIdleUnits)
        {
            Debug.Log($"[DEBUG] Кандидат: {unit.name}, база: {unit.GetAssignedBase()?.name}, свободен: {unit.ReadyForNewTask}");
        }

        Unit builder = ownIdleUnits.FirstOrDefault();

        if (builder == null)
        {
            Debug.Log("[BaseExpansion] Юнитов нет, ждём дальше...");
            _waitingForBuilder = true;
            return;
        }

        if (_counter.Decrement(_resourcesToExpand) == false)
        {
            Debug.LogWarning("[BaseExpansion] Race condition: ресурсов не хватило на Decrement.");
            return;
        }

        Debug.Log($"[BaseExpansion] Assigned unit {builder.name} to build base at {_flagPosition.Value}");
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
        if (distance > 7f)
        {
            Debug.Log($"[BaseExpansion] Unit {unit.name} is too far from flag to build. Distance: {distance}");
            return;
        }

        Debug.Log($"[BaseExpansion] Unit {unit.name} arrived. Building new base at {_flagPosition.Value}");

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
