using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 100f;
    [SerializeField] private Vector2 _limitX = new Vector2(-500f, 500f);
    [SerializeField] private Vector2 _limitZ = new Vector2(-500f, 500f);
    [SerializeField] private Transform _referenceTransform;

    private Vector2 _defoultInput = Vector2.zero;

    public void ChangePosition(Vector2 input)
    {
        if (input == _defoultInput) 
            return;

        Vector3 forward = _referenceTransform.forward;
        Vector3 right = _referenceTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (right * input.x + forward * input.y).normalized;

        Vector3 move = moveDir * _moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + move;

        newPosition.x = Mathf.Clamp(newPosition.x, _limitX.x, _limitX.y);
        newPosition.z = Mathf.Clamp(newPosition.z, _limitZ.x, _limitZ.y);

        transform.position = newPosition;
    }
}
