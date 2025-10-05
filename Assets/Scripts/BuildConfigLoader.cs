using System.Collections;
using System.IO;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Networking;
#endif

public static class BuildConfigLoader
{
    public static BuildConfig Config { get; private set; }
    public static bool IsLoaded { get; private set; }
    
    public static IEnumerator LoadConfig()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "BuildConfig.json");
        string json = null;

#if UNITY_ANDROID && !UNITY_EDITOR
        // Android: StreamingAssets are inside APK
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
            json = request.downloadHandler.text;
        else
            Debug.LogError($"[BuildConfigLoader] Failed to load config: {request.error}");
#else
        if (File.Exists(path))
            json = File.ReadAllText(path);
        else
            Debug.LogError($"[BuildConfigLoader] Missing file: {path}");
#endif

        if (!string.IsNullOrEmpty(json))
        {
            Config = JsonUtility.FromJson<BuildConfig>(json);
            IsLoaded = true;
            Debug.Log($"[BuildConfigLoader] Loaded ApiHost: {Config.ApiHost}");
        }
        yield break;
    }
}