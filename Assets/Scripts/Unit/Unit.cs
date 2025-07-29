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

    public bool ReadyForNewTask { get; private set; }
    public bool IsBusy { get; private set; }

    public event Action<Unit, Resource> ResourceDelivered;
    public event Action<Unit> OnArrived;

    public void Initialize(Vector3 position)
    {
        _basePosition = position;
        IsBusy = false;
        ReadyForNewTask = true;
    }

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

    private void HandleArrived()
    {
        OnArrived?.Invoke(this);
    }

    public void MoveTo(Vector3 position)
    {
        IsBusy = true;
        ReadyForNewTask = false;
        _mover.SetTarget(position);
    }

    public bool SetTarget(Resource resource)
    {
        if (resource == null)
        {
            return false;
        }

        if (IsBusy)
        {
            return false;
        }

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
    }

    public void AssignToBase(Base baseRef)
    {
        _assignedBase = baseRef;
    }

    public Base GetAssignedBase()
    {
        return _assignedBase;
    }
}
