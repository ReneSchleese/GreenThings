using System.Collections;
using System.Collections.Generic;
using Audio;
using ForestSpirits;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private AudioClip _ambientClip;
    private List<ForestSpiritSpawn> _forestSpiritSpawns = new();
    private static Game _instance;

    public void Awake()
    {
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        // wait for spawners to register
        yield return null;
        
        SpawnForestSpirits();
        AudioManager.Instance.PlayAmbient(_ambientClip, loop: true);
    }

    private void SpawnForestSpirits()
    {
        
    }

    public void Register(ForestSpiritSpawn spawn)
    {
        Debug.Log($"Register {spawn}");
        _forestSpiritSpawns.Add(spawn);
    }

    public static Game Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Game>();
            }
            return _instance;
        }
    }
}