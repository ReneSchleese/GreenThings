using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UserData
{
    public event Action Update;
    private static string _filePath;

    public UserData()
    {
        _filePath = Path.Combine(Application.persistentDataPath, "userData.json");
    }

    public void Load()
    {
        if (!File.Exists(_filePath))
        {
            return;
        }
        string userDataText = File.ReadAllText(_filePath);
        UserDataJson userDataJson = JsonUtility.FromJson<UserDataJson>(userDataText);
        Money = userDataJson.money;
        OwnedMessageIds = userDataJson.ownedMessageIds.ToHashSet();
        Update?.Invoke();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(new UserDataJson()
        {
            money = Money,
            ownedMessageIds = OwnedMessageIds.ToArray(),
        });
        File.WriteAllText(_filePath, json);
    }

    public void Buy(BottledMessageJson message)
    {
        if (Money < message.price) return;
        OwnedMessageIds.Add(message.id);
        Money = Mathf.Max(0,  Money - message.price);
        Save();
        Update?.Invoke();
    }

    public int Money { get; set; }
    public HashSet<string> OwnedMessageIds { get; private set; } = new();
}