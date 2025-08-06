using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class UnitMover : MonoBehaviour
{
    [SerializeField] private float _baseSpeed = 25f;
    [SerializeField] private float _arriveThreshold = 4f; 

    private Rigidbody _rigidbody;
    private Vector3 _targetPosition;
    private Vector3 _moveDirection;

    private bool _hasTarget = false;
    private float _yVelocity = 0f;

    public event Action OnArrived;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetTarget(Vector3 position)
    {
        _targetPosition = position;
        _hasTarget = true;
    }

    public void ClearTarget()
    {
        _hasTarget = false;
    }

    private void Move()
    {
        if (_hasTarget == false)
            return;

        float distance = Vector3.Distance(_rigidbody.position, _targetPosition);

        if (distance < _arriveThreshold)
        {
            _hasTarget = false;
            OnArrived?.Invoke();
            return;
        }

        _moveDirection = (_targetPosition - transform.position).normalized;

        Vector3 velocity = _moveDirection * _baseSpeed;
        Vector3 nextPosition = _rigidbody.position + new Vector3(velocity.x, _yVelocity, velocity.z) * Time.fixedDeltaTime;

        _rigidbody.MovePosition(nextPosition);
    }
}
