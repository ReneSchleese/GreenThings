using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IAppState
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _shopButton;

    private void Awake()
    {
        App.Instance.NotifyAwakeAppState(this);
    }

    void Start()
    {
        _startGameButton.onClick.AddListener(OnStartGamePressed);
        _shopButton.onClick.AddListener(OnShopPressed);

        void OnStartGamePressed()
        {
            App.Instance.TransitionToGame();
        }

        void OnShopPressed()
        {
            StartCoroutine(Download());
            IEnumerator Download()
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
            }
        }
    }

    public IEnumerator TransitionOut()
    {
        Debug.Log("MainMenu.TransitionOff");
        yield break;
    }

    public IEnumerator TransitionIn()
    {
        Debug.Log("MainMenu.TransitionTo");
        yield break;
    }

    public void OnUnload()
    {
        Debug.Log("MainMenu.OnUnload");
    }

    public IEnumerator OnLoad()
    {
        Debug.Log("MainMenu.OnLoadComplete");
        yield break;
    }

    public AppState Id => AppState.MainMenu;
}
