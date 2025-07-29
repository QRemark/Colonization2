using UnityEngine;

public class UnitResourceHandler : MonoBehaviour
{
    [SerializeField] private float _pickupHeightOffset = 6f;
    [SerializeField] private float _deliveryRadius = 2f;

    private Resource _carriedResource;

    public void SetCarriedResource(Resource resource)
    {
        _carriedResource = resource;
    }

    public void ClearCarryState()
    {
        _carriedResource = null;
    }

    public bool IsTryPickup(Resource target, float pickupRadius)
    {
        if (Vector3.Distance(transform.position, target.transform.position) > pickupRadius)
            return false;

        target.MarkCollected();
        _carriedResource = target;

        if (_carriedResource.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = true;
        }

        _carriedResource.transform.SetParent(transform);
        _carriedResource.transform.localPosition = new Vector3(0, _pickupHeightOffset, 0);

        return true;
    }

    public bool IsTryDelivery(Vector3 deliveryPoint, out Resource delivered)
    {
        delivered = null;

        if (_carriedResource == null)
            return false;

        float distance = Vector3.Distance(transform.position, deliveryPoint);

        if (distance > _deliveryRadius)
            return false;

        _carriedResource.transform.SetParent(null);

        if (_carriedResource.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = false;
        }

        delivered = _carriedResource;
        _carriedResource = null;

        return true;
    }
}