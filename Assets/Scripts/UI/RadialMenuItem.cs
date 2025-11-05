using TMPro;
using UnityEngine;

public class RadialMenuItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;

    public void Init(string label)
    {
        _label.text = label;
    }
}