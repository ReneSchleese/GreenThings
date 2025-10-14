using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadableContent : MonoBehaviour
{
    private readonly Dictionary<string, Texture2D> _textures = new();
    private readonly Dictionary<string, List<Action<Texture2D>>> _textureRequests = new();

    public void GetTexture(string url, Action<Texture2D> callback)
    {
        Debug.Log("GetTexture: " + url);
        bool alreadyInMemory = _textures.TryGetValue(url, out var cached);
        if (alreadyInMemory)
        {
            Debug.Log("alreadyInMemory: " + url);
            callback?.Invoke(cached);
            return;
        }

        bool alreadyDownloading = _textureRequests.TryGetValue(url, out var list);
        if (alreadyDownloading)
        {
            Debug.Log("alreadyDownloading: " + url);
            list.Add(callback);
            return;
        }

        _textureRequests[url] = new List<Action<Texture2D>> { callback };
        StartCoroutine(LoadFromDiskDownloadOtherwise(url));
    }

    private IEnumerator LoadFromDiskDownloadOtherwise(string url)
    {
        string tempFilePath = ToLocalFilePath(url);
        CheckDirectory(tempFilePath);
        Debug.Log("tempFilePath: " + tempFilePath);

        if (!File.Exists(tempFilePath))
        {
            using (var webRequest = UnityWebRequest.Get(url))
            {
                webRequest.SetRequestHeader("x-api-key", BuildConfigLoader.Config.ApiKey);
                Debug.Log("downloading: " + url);
                yield return webRequest.SendWebRequest();
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"Failed to download {url}: {webRequest.error}");
                    CallbackRequests(url, null);
                    yield break;
                }
                File.WriteAllBytes(tempFilePath, webRequest.downloadHandler.data);
            }
        }

        // Load into memory (example for Texture2D)
        Texture2D texture = LoadFromFile(tempFilePath);

        _textures[url] = texture;
        CallbackRequests(url, texture);
    }

    private static string ToLocalFilePath(string url)
    {
        Debug.Assert(BuildConfigLoader.IsLoaded, "BuildConfigLoader.IsLoaded");
        string schemeWithHost = $"https://{BuildConfigLoader.Config.ApiHost}/";
        return Path.Combine(Application.temporaryCachePath, url.Replace(schemeWithHost, ""));
    }

    private void CheckDirectory(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath)?.Replace("\\", "/");
        Debug.Assert(directory != null, "directory != null");
        Debug.Log($"CheckDirectory={directory}");
        Directory.CreateDirectory(directory);
    }

    private Texture2D LoadFromFile(string filePath)
    {
        Debug.Log("loadFromFile: " + filePath);
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        bool success = tex.LoadImage(File.ReadAllBytes(filePath));

        if (!success)
        {
            Debug.LogError("Failed to load texture from file!");
        }

        return tex;
    }
    
    private void CallbackRequests(string url, Texture2D texture)
    {
        if (!_textureRequests.TryGetValue(url, out List<Action<Texture2D>> requests))
        {
            return;
        }
        foreach (Action<Texture2D> request in requests)
        {
            // TODO: requester might be destroyed, need to check
            request?.Invoke(texture);
        }
        _textureRequests.Remove(url);
    }
}