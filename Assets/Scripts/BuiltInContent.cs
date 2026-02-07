using System;
using System.Collections.Generic;
using UnityEngine;

public class BuiltInContent : MonoBehaviour
{
    [SerializeField] private List<VinylData> _vinyls;
    private Dictionary<VinylId, VinylData> _vinylsDict;

    public void Init()
    {
        _vinylsDict = new Dictionary<VinylId, VinylData>();
        foreach (VinylData vinylData in _vinyls)
        {
            _vinylsDict.Add(vinylData.Id, vinylData);
        }
    }
    
    public VinylData GetVinylData(VinylId id) => _vinylsDict[id];
}

[Serializable]
public class VinylData
{
    public VinylId Id;
    public AudioClip Clip;
    public Texture2D Thumbnail;
    public string Title;
}