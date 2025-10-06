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
    private Coroutine _requestRoutine;

    public void Fetch(BuildConfig config)
    {
        if (_requestRoutine != null)
        {
            Debug.LogError("Shop connection is already fetching");
            return;
        }

        _requestRoutine = StartCoroutine(Download(config));
    }

    private IEnumerator Download(BuildConfig config)
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
                // Get the JSON response as a string
                string json = request.downloadHandler.text;
                Debug.Log("Received JSON:\n" + json);

                //var messages = JsonUtility.FromJson<MessageList>(json);
                State = RequestState.Success;
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