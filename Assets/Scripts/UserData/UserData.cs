using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UserData
{
    private static string _filePath;
    private List<string> _ownedMessageIds = new();

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
        _ownedMessageIds = userDataJson.ownedMessageIds.ToList();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(new UserDataJson()
        {
            money = Money,
            ownedMessageIds = _ownedMessageIds.ToArray(),
        });
        File.WriteAllText(_filePath, json);
    }

    public int Money { get; set; }
}