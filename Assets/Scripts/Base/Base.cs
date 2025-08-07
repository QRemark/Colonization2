using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private BaseFlow _flow;
    [SerializeField] private UnitProduction _unitProducion;
    [SerializeField] private BaseExpansion _baseExpansion;
    [SerializeField] private ResourceCounter _resourceCounter;

    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _highlightColor = Color.red;

    private bool _isInExpansionMode = false;
    private bool _canProduceWhileExpanding = false;

    public bool CanProduceUnits => _isInExpansionMode == false|| _canProduceWhileExpanding;

    private void Awake()
    {
        _renderer.material = new Material(_renderer.material);
    }

    public void Init(UnitSpawner unitSpawner, ResourceStorage resourceStorage, GlobalUnitHandler globalUnitHandler, GlobalBaseHandler baseHandler)
    {
        _flow.Init(unitSpawner, resourceStorage, this);

        _unitProducion.Init(this, _resourceCounter, globalUnitHandler);
        _baseExpansion.Init(this, _resourceCounter, globalUnitHandler, baseHandler);
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

        _unitProducion.enabled = CanProduceUnits;
    }

    public void NotifyBuilderSent()
    {
        _canProduceWhileExpanding = true;
        _unitProducion.enabled = true;
    }

    public void SetFlag(Vector3 position)
    {
        _baseExpansion.SetFlag(position);
    }

    public void SubscribeToUnit(Unit unit)
    {
        unit.BecameIdle += HandleUnitBecameIdle;
    }

    public void UnsubscribeFromUnit(Unit unit)
    {
        unit.BecameIdle -= HandleUnitBecameIdle;
    }

    public void OnResourceCountChanged(int count)
    {
        _baseExpansion.OnResourceCountChanged(count);
    }

    private void HandleUnitBecameIdle(Unit unit)
    {
        if (_isInExpansionMode)
        {
            _baseExpansion.OnUnitIdle(unit);
        }
    }
}
