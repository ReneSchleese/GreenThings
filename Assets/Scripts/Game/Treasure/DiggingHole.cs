using System;
using DG.Tweening;
using UnityEngine;

public class DiggingHole : MonoBehaviour
{
    private int _timesBeingDug;
    private const int MAX_TIMES = 3;
    public void OnBeingDug()
    {
        if (_timesBeingDug == 0)
        {
            transform.localScale = Vector3.zero;
        }
        _timesBeingDug = Math.Clamp(_timesBeingDug + 1, 0, MAX_TIMES);
        transform.DOScale(new Vector3(0.5f * _timesBeingDug, 0.5f, 0.5f * _timesBeingDug), 0.3f).SetEase(Ease.InOutCubic);
    }
}