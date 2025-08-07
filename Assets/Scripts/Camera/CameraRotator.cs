using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 90f;
    
    private float _zeroRotate = 0f;
    private float _zeroAngle = 0f;

    public void Rotate(float input)
    {
        if (Mathf.Approximately(input, _zeroAngle)) 
            return;

        float angleY = input * _rotationSpeed * Time.deltaTime;
        transform.Rotate(_zeroRotate, angleY, _zeroRotate);
    }
}
