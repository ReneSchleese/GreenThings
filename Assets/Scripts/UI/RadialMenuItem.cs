using TMPro;
using UnityEngine;

public class RadialMenuItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private RectTransform _rectTransform;

    public void Init(string label)
    {
        _label.text = label;
    }

    public RectTransform RectTransform => _rectTransform;
}