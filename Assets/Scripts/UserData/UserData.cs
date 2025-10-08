using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UserData
{
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

    public int Money { get; set; }
    public HashSet<string> OwnedMessageIds { get; private set; } = new();
}