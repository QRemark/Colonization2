using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private BaseFlow _flow;
    [SerializeField] private UnitExpansion _unitExpansion;
    [SerializeField] private BaseExpansion _baseExpansion;
    [SerializeField] private ResourceCounter _resourceCounter;

    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _highlightColor = Color.red;

    private bool _isInExpansionMode = false;
    private bool _canProduceWhileExpanding = false;

    public bool IsInExpansionMode => _isInExpansionMode;
    public bool CanProduceUnits => _isInExpansionMode == false|| _canProduceWhileExpanding;

    private void Awake()
    {
        _renderer.material = new Material(_renderer.material);
    }

    public void Init(UnitSpawner unitSpawner, ResourceStorage resourceStorage, GlobalUnitHandler globalUnitHandler, BaseManager baseManager)
    {
        _flow.Init(unitSpawner, resourceStorage, this);

        _unitExpansion.Init(this, _resourceCounter, globalUnitHandler);
        _baseExpansion.Init(this, _resourceCounter, globalUnitHandler, baseManager);
    }

    public void SetHighlight(bool isHighlighted)
    {
        if (_renderer != null)
        {
            _renderer.material.color = isHighlighted ? _highlightColor : _defaultColor;
        }
    }

    public void SetExpansionMode(bool isExpanding)
    {
        _isInExpansionMode = isExpanding;

        if (isExpanding == false)
            _canProduceWhileExpanding = false;

        _unitExpansion.enabled = CanProduceUnits;
    }
    public void NotifyBuilderSent()
    {
        Debug.Log("[Base] Юнит отправлен, разрешаем производство во время расширения.");
        _canProduceWhileExpanding = true;
        _unitExpansion.enabled = true;
    }

    public void SetFlag(Vector3 position)
    {
        _baseExpansion.SetFlag(position);
    }
    public void OnUnitSpawned(Unit unit)
    {
        unit.BecameIdle += HandleUnitBecameIdle;
    }

    private void HandleUnitBecameIdle(Unit unit)
    {
        Debug.Log($"[Base] Пойман BecameIdle от {unit.name}, база {_baseExpansion.name}, isInExpansionMode: {_isInExpansionMode}");

        if (_isInExpansionMode)
        {
            _baseExpansion.OnUnitIdleFromThisBase(unit);
        }
    }
    public void OnResourceCountChanged(int count)
    {
        _baseExpansion.OnResourceCountChanged(count);
    }

}
