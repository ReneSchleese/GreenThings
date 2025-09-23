using System.Collections;
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
        string savePath = Path.Combine(Application.temporaryCachePath, "testVideo.mp4");
        using UnityWebRequest request = new(url, UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = new DownloadHandlerFile(savePath);

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
            Debug.Log("Download complete! Saved to: " + savePath);
        }

        _downloadRoutine = null;
    }
}