using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Path = System.IO.Path;

public static class FilesManager
{
    private static Coroutine _downloadRoutine;

    public static void HandleFile()
    {
        if (_downloadRoutine != null)
        {
            Debug.LogWarning("ongoing download");
            return;
        }

        _downloadRoutine = App.Instance.StartCoroutine(DownloadFile());
    }

    private static IEnumerator DownloadFile()
    {
        string url = "https://test-videos.co.uk/vids/bigbuckbunny/mp4/h264/1080/Big_Buck_Bunny_1080_10s_1MB.mp4";
        string filePath = Path.Combine(Application.temporaryCachePath, "testVideo.mp4");
        using UnityWebRequest request = new(url, UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = new DownloadHandlerFile(filePath);

        request.SendWebRequest();
        
        while (!request.isDone)
        {
            Debug.Log($"Download progress: {request.downloadProgress * 100f}%");
            yield return null;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Download failed: " + request.error);
        }
        else
        {
            Debug.Log("Download complete! Saved to: " + filePath);
        }

        _downloadRoutine = null;
        
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            yield break;
        }

        try
        {
            using AndroidJavaClass intentClass = new("android.content.Intent");
            using AndroidJavaObject intentObject = new("android.content.Intent");
            // Set action to send
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

            // Get context
            AndroidJavaClass unity = new("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            // File + URI
            AndroidJavaObject fileObject = new("java.io.File", filePath);
            AndroidJavaClass fileProviderClass = new("androidx.core.content.FileProvider");
            string authority = currentActivity.Call<string>("getPackageName") + ".fileprovider";
            AndroidJavaObject uriObject = fileProviderClass.CallStatic<AndroidJavaObject>(
                "getUriForFile", currentActivity, authority, fileObject);

            // Add extras
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("setType", "video");

            // Grant read permission
            int flagGrantReadUriPermission = intentClass.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION");
            intentObject.Call<AndroidJavaObject>("addFlags", flagGrantReadUriPermission);

            // Show chooser
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>(
                "createChooser", intentObject, "Share Video");
            currentActivity.Call("startActivity", chooser);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Share failed: " + e);
        }
    }
}