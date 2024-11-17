using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [SerializeField] private ForestSpirits.Spirit _forestSpiritPrefab;
    [SerializeField] private Transform _forestSpiritParent;
    
    public void SpawnForestSpirit(Vector3 position, Quaternion rotation)
    {
        Instantiate(_forestSpiritPrefab, position, rotation, _forestSpiritParent);
    }
}