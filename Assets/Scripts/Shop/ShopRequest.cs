using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ShopRequest : MonoBehaviour
{
    public enum RequestState
    {
        Unknown,
        Fetching,
        Success,
        Failure
    }
    
    public event Action<RequestState> OnStateChange;
    public event Action<BottledMessagesJson> OnMessagesFetchComplete;
    private Coroutine _requestRoutine;

    public void Fetch(BuildConfig config)
    {
        if (_requestRoutine != null)
        {
            Debug.LogError("Shop connection is already fetching");
            return;
        }

        _requestRoutine = StartCoroutine(GetMessages(config));
    }

    private IEnumerator GetMessages(BuildConfig config)
    {
        State = RequestState.Fetching;
        OnStateChange?.Invoke(State);
        
        UriBuilder uriBuilder = new UriBuilder
        {
            Scheme = "https",
            Host = config.ApiHost,
            Path = "api/bottled-messages",
        };
        UnityWebRequest request = UnityWebRequest.Get(uriBuilder.Uri);
        request.SetRequestHeader("x-api-key", config.ApiKey);

        yield return request.SendWebRequest();
                
        if (request.result != UnityWebRequest.Result.Success)
        {
            State = RequestState.Failure;
            OnStateChange?.Invoke(State);
            Debug.LogError($"Request failed: {request.error}");
        }
        else
        {
            try
            {
                string json = request.downloadHandler.text;
                Debug.Log($"[{nameof(ShopRequest)}.{nameof(GetMessages)}] Received JSON:\n" + json);
                json = $"{{ \"messages\" : {json} }}";
                BottledMessagesJson messagesJson = JsonUtility.FromJson<BottledMessagesJson>(json);
                State = RequestState.Success;
                OnMessagesFetchComplete?.Invoke(messagesJson);
            }
            catch (Exception e)
            {
                State = RequestState.Failure;
                Debug.LogError(e);
            }
            
            OnStateChange?.Invoke(State);
        }

        _requestRoutine = null;
    }

    public RequestState State { get; private set; }
}