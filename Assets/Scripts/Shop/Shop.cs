using System.Collections.Generic;
using System.Linq;

public class Shop
{
    public void Init()
    {
        App.Instance.ShopRequest.OnMessagesFetchComplete += OnMessagesFetchComplete;
    }

    private void OnMessagesFetchComplete(BottledMessagesJson messagesJson)
    {
        Messages = messagesJson.messages.ToList();
    }

    public List<BottledMessageJson> Messages { get; private set; } = new();
}