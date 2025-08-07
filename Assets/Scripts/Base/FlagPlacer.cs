using UnityEngine;

public class FlagPlacer : MonoBehaviour
{
    [SerializeField] private GameObject _flagPrefab;

    private static readonly Quaternion _defaultRotation = Quaternion.identity;

    private GameObject _flagInstance;
    private Vector3? _flagPosition;

    public Vector3? FlagPosition => _flagPosition;
    public bool HasFlag => _flagPosition != null;

    public void PlaceFlag(Vector3 position)
    {
        ClearFlag();

        _flagInstance = Instantiate(_flagPrefab, position, _defaultRotation);
        _flagPosition = position;
    }

    public void ClearFlag()
    {
        if (_flagInstance != null)
        {
            Destroy(_flagInstance);
            _flagInstance = null;
        }

        _flagPosition = null;
    }
}
