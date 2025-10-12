using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Shop
{
    public event Action Update;
    private static string _cachedShopFilePath;

    public Shop()
    {
        _cachedShopFilePath = Path.Combine(Application.persistentDataPath, "cached-shop.json");
    }
    
    public void Init()
    {
        App.Instance.ShopRequest.OnMessagesFetchComplete += OnMessagesFetchComplete;
    }

    public void LoadFromCache()
    {
        if (!File.Exists(_cachedShopFilePath))
        {
            Debug.LogWarning("No cached shop found. Is this the first launch?");
            return;
        }
        string cachedShopJson = File.ReadAllText(_cachedShopFilePath);
        BottledMessagesJson cachedShopJsonObj = JsonUtility.FromJson<BottledMessagesJson>(cachedShopJson);
        Messages = cachedShopJsonObj.messages.ToList();
    }

    private void SaveCurrentToCache()
    {
        var messagesJsonObj = new BottledMessagesJson()
        {
            messages = Messages.ToArray()
        };
        string messagesJson = JsonUtility.ToJson(messagesJsonObj);
        File.WriteAllText(_cachedShopFilePath, messagesJson);
    }

    private void OnMessagesFetchComplete(BottledMessagesJson messagesJson)
    {
        Messages = messagesJson.messages.ToList();
        Update?.Invoke();
        SaveCurrentToCache();
    }

    public List<BottledMessageJson> Messages { get; private set; } = new();
}