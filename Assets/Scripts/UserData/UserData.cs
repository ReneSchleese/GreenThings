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
        OwnedVinylIds = userDataJson.ownedVinylIds == null
            ? new HashSet<VinylId>()
            : userDataJson.ownedVinylIds
                .Select(enumString => Enum.TryParse<VinylId>(enumString, ignoreCase: true, out var vinylId)
                    ? (VinylId?)vinylId
                    : null)
                .Where(vinylId => vinylId.HasValue)
                .Select(vinylId => vinylId.Value)
                .ToHashSet();
        Update?.Invoke();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(new UserDataJson
        {
            money = Money,
            ownedMessageIds = OwnedMessageIds.ToArray(),
            ownedVinylIds = OwnedVinylIds.Select(vinylId => vinylId.ToString()).ToArray()
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
    public HashSet<VinylId> OwnedVinylIds { get; private set; } = new();
}