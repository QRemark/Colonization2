using TMPro;
using UnityEngine;

public class BaseResourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField]private float _maxWriteCount = 99f;

    public void Initialize(ResourceCounter counter)
    {
        counter.CountChanged += UpdateText;
    }

    private void UpdateText(int count)
    {
        if (count > _maxWriteCount)
        {
            _text.text = $"{_maxWriteCount}+";
        }
        else
        {
            _text.text = count.ToString();
        }
    }
}
