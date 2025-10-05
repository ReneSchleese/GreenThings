using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildConfigWriter : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        WriteBuildConfig();
    }
    
    [MenuItem("Build/Write Build Config (Manual)")]
    public static void WriteBuildConfig()
    {
        string apiUrl = Environment.GetEnvironmentVariable("GREEN_THINGS_API_HOST");
        string apiKey = Environment.GetEnvironmentVariable("GREEN_THINGS_API_KEY");

        var config = new BuildConfig
        {
            ApiHost = apiUrl,
            ApiKey = apiKey,
            BuildTime = DateTime.UtcNow.ToString("u")
        };
        
        string dir = Path.Combine(Application.dataPath, "StreamingAssets");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, "BuildConfig.json");
        
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(path, json);
        Debug.Log($"[BuildConfigWriter] Wrote build config to {path}");
    }
}