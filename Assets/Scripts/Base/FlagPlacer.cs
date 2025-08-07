using UnityEngine;

public class FlagPlacer : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;

    private static readonly Quaternion _defaultRotation = Quaternion.identity;

    private Flag _flagInstance;
    private Vector3? _flagPosition;

    private Vector3 _defoultPlace = Vector3.zero;

    public Vector3? FlagPosition => _flagPosition;
    public bool HasFlag => _flagPosition != null;

    private void Awake()
    {
        _flagInstance = Instantiate(_flagPrefab, _defoultPlace, _defaultRotation);
        _flagInstance.gameObject.SetActive(false);
    }

    public void Place(Vector3 position)
    {
        _flagPosition = position;

        _flagInstance.transform.SetPositionAndRotation(position, _defaultRotation);
        _flagInstance.gameObject.SetActive(true);
    }

    public void Clear()
    {
        if (_flagInstance != null)
        {
            _flagInstance.gameObject.SetActive(false);
        }

        _flagPosition = null;
    }
}
