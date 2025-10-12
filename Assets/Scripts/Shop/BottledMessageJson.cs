using System;

// ReSharper disable InconsistentNaming

[Serializable]
public class BottledMessageJson
{
    public string title;
    public string id;
    public string content_url;
    public string thumbnail_url;
    public int price;
    public string time_created;
}