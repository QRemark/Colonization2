using System;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]
[RequireComponent(typeof(UnitResourceHandler))]
public class Unit : MonoBehaviour
{
    [SerializeField] private float _pickupRadius = 10f;

    private UnitMover _mover;
    private UnitResourceHandler _resourceHandler;

    private Vector3 _basePosition;
    private Resource _targetResource;
    private Base _assignedBase;

    private bool _isBuildingBase = false;

    public bool ReadyForNewTask { get; private set; }
    public bool IsBusy { get; private set; }

    public event Action<Unit, Resource> ResourceDelivered;
    public event Action<Unit> OnArrived;
    public event Action<Unit> BecameIdle;

    private void Awake()
    {
        _mover = GetComponent<UnitMover>();
        _resourceHandler = GetComponent<UnitResourceHandler>();

        _mover.OnArrived += HandleArrived;
    }

    private void FixedUpdate()
    {
        if (IsBusy == false)
            return;

        if (_isBuildingBase)
            return;

        if (_resourceHandler.IsTryPickup(_targetResource, _pickupRadius))
        {
            _mover.SetTarget(_basePosition);
        }

        if (_resourceHandler.IsTryDelivery(_basePosition, out Resource delivered))
        {
            NotifyDelivery(delivered);
            BecomeIdle();
        }
    }

    public void Initialize(Vector3 position)
    {
        _basePosition = position;
        IsBusy = false;
        ReadyForNewTask = true;
    }

    public void StartBaseBuildingTask(Vector3 position)
    {
        _isBuildingBase = true;
        IsBusy = true;
        ReadyForNewTask = false;

        _targetResource = null;
        _resourceHandler.ClearCarryState();
        _mover.SetTarget(position);
    }

    public bool SetTarget(Resource resource)
    {
        if (resource == null || IsBusy)
            return false;

        _targetResource = resource;
        _mover.SetTarget(resource.transform.position);

        _resourceHandler.SetCarriedResource(null);
        _resourceHandler.ClearCarryState();

        IsBusy = true;
        ReadyForNewTask = false;

        return true;
    }

    public void NotifyDelivery(Resource delivered)
    {
        ResourceDelivered?.Invoke(this, delivered);
    }

    public void BecomeIdle()
    {
        _targetResource = null;
        IsBusy = false;
        ReadyForNewTask = true;
        _mover.ClearTarget();
        BecameIdle?.Invoke(this);
    }

    public void AssignToBase(Base baseRef)
    {
        _assignedBase = baseRef;
    }

    public Base GetAssignedBase()
    {
        return _assignedBase;
    }

    private void HandleArrived()
    {
        if (_isBuildingBase)
            _isBuildingBase = false;

        OnArrived?.Invoke(this);
    }
}
