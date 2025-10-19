using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadableContent : MonoBehaviour
{
    private readonly Dictionary<string, Texture2D> _textures = new();

    public event Action<string, Texture2D> TextureIsReady;
    public event Action<string> FailedToLoad;

    public void RequestTexture(string url)
    {
        Debug.Log($"{nameof(RequestTexture)}: " + url);
        bool alreadyInMemory = _textures.TryGetValue(url, out var texture);
        if (alreadyInMemory)
        {
            TextureIsReady?.Invoke(url, texture);
            return;
        }

        StartCoroutine(DownloadThenLoadIntoMemory());
        return;

        IEnumerator DownloadThenLoadIntoMemory()
        {
            string localFilePath = ToLocalFilePath(url);
            yield return DownloadAndSaveTo(url, localFilePath);
            if(!_textures.TryGetValue(url, out texture))
            {
                texture = LoadFromFile(url, localFilePath);
                _textures[url] = texture;
            }
            TextureIsReady?.Invoke(url, texture);
        }
    }

    private IEnumerator DownloadAndSaveTo(string url, string localFilePath)
    {
        CreateDirectoryIfItDoesntExist(localFilePath);

        if (File.Exists(localFilePath))
        {
            yield break;
        }
        using (var webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("x-api-key", BuildConfigLoader.Config.ApiKey);
            Debug.Log("downloading: " + url);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Failed to download {url}: {webRequest.error}");
                FailedToLoad?.Invoke(url);
                yield break;
            }
            File.WriteAllBytes(localFilePath, webRequest.downloadHandler.data);
        }
    }

    private static string ToLocalFilePath(string url)
    {
        Debug.Assert(BuildConfigLoader.IsLoaded, "BuildConfigLoader.IsLoaded");
        string schemeWithHost = $"https://{BuildConfigLoader.Config.ApiHost}/";
        return Path.Combine(Application.temporaryCachePath, url.Replace(schemeWithHost, ""));
    }

    private void CreateDirectoryIfItDoesntExist(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath)?.Replace("\\", "/");
        Debug.Assert(directory != null, "directory != null");
        Directory.CreateDirectory(directory);
    }

    private Texture2D LoadFromFile(string url, string filePath)
    {
        Debug.Log("loadFromFile: " + filePath);
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        bool success = tex.LoadImage(File.ReadAllBytes(filePath));

        if (!success)
        {
            FailedToLoad?.Invoke(url);
        }

        return tex;
    }
}