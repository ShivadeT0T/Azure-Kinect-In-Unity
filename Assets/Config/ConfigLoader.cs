using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigLoader : MonoBehaviour
{
    public static ConfigLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        LoadSceneSetup();
    }

    // Name of scene config file.
    private const string gameDataFileName = "config.json";

    // Placeholder for animation json file
    private const string animationJsonFile = "test2.json";

    public Configs Configs { get; private set; } = new Configs();
    public List<BackgroundDataNoDepth> Frames { get; private set; } = new List<BackgroundDataNoDepth>();

    public string animationJson;

    private void LoadSceneSetup()
    {
        // Path.Combine combines strings into a file path.
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build.
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

        // Placeholder path

        string animationPath = Path.Combine(Application.streamingAssetsPath, animationJsonFile);

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string.
            string dataAsJson = File.ReadAllText(filePath);

            // Pass the json to JsonUtility, and tell it to create a Configs object from it.
            Configs = JsonUtility.FromJson<Configs>(dataAsJson);

            UnityEngine.Debug.Log("Successfully loaded config file.");
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }

        if (File.Exists(animationPath))
        {
                ITraceWriter traceWriter = new MemoryTraceWriter();
            try
            {
                animationJson = File.ReadAllText(animationPath);
                Debug.Log(animationJson);
                Frames = JsonConvert.DeserializeObject<List<BackgroundDataNoDepth>>(animationJson, new JsonSerializerSettings { TraceWriter = traceWriter});

            } catch(Exception e)
            {
                Debug.Log(traceWriter);
                Debug.Log($"Error in line 71: {e.Message}");
            }

            UnityEngine.Debug.Log("Animation load success");
        }
        else
        {
            Debug.LogError("Cannot load animation data!");
        }
    }
}
