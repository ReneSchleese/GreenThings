using UnityEngine;

public enum SurfaceType
{
    Grass,
    Stone
}

public class EnvironmentObject : MonoBehaviour
{
    [SerializeField] private SurfaceType _surfaceType;
    [SerializeField] private bool _allowsTreasureSpawn;
    public SurfaceType SurfaceType => _surfaceType;
    public bool AllowsTreasureSpawn => _allowsTreasureSpawn && _surfaceType == SurfaceType.Grass;
}
