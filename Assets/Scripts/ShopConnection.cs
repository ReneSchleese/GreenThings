using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ShopConnection : MonoBehaviour
{
    private Coroutine _requestRoutine;

    public void Fetch()
    {
        if (IsFetching)
        {
            Debug.LogError("Shop connection is already fetching");
            return;
        }

        _requestRoutine = StartCoroutine(Download());
    }

    private IEnumerator Download()
    {
        // TODO: must be included in build
        string apiKey = System.Environment.GetEnvironmentVariable("GREEN_THINGS_API_KEY");
        string host = System.Environment.GetEnvironmentVariable("GREEN_THINGS_API_HOST");
        Debug.Log($"apiKey={apiKey}, host={host}");
        UnityWebRequest request = UnityWebRequest.Get($"https://{host}/api/bottled-messages");
        request.SetRequestHeader("x-api-key", apiKey);

        yield return request.SendWebRequest();
                
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Request failed: {request.error}");
        }
        else
        {
            // Get the JSON response as a string
            string json = request.downloadHandler.text;

            Debug.Log("Received JSON:\n" + json);

            // Optionally parse it into a class/array
            // Example: Deserialize into your data model
            // var messages = JsonUtility.FromJson<MessageList>(json);
        }

        _requestRoutine = null;
    }
    
    public bool IsFetching => _requestRoutine != null;
}