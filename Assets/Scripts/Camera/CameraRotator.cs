using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 90f;

    public void Rotate(float input)
    {
        if (Mathf.Approximately(input, 0f)) 
            return;

        float angleY = input * _rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, angleY, 0f);
    }
}
