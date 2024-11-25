
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileManager
{
    public static void CreateJsonFile(string fileName, string json)
    {
        try
        {
            string directory = Application.streamingAssetsPath + Path.DirectorySeparatorChar +  "Animations";
            string jsonFile = fileName + ".json";

            string filePath = Path.Combine(directory, jsonFile);
            File.WriteAllText(filePath, json);
            Debug.Log("Writing JSON file successful");
        } catch (Exception e)
        {
            Debug.LogError($"Failed to write to a file: {e.Message}");
        }
    }

    public static string LoadJsonFile(string fileName)
    {

        string animationJson = null;
        try
        {
            string directory = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Animations";
            string jsonFile = fileName + ".json";

            string filePath = Path.Combine(directory, jsonFile);
            animationJson = File.ReadAllText(filePath);
            Debug.Log("Loading JSON file successful");

        } catch (Exception e)
        {
            Debug.LogError($"Couldn't load JSON file: {e.Message}");
        }
            return animationJson;
    }

    public static IList<AnimationFile> LoadFilesInfo()
    {
        List<AnimationFile> animationFiles = new List<AnimationFile>();
        string animationDirectory = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Animations";
        try
        {
            var jsonFiles = Directory.EnumerateFiles(animationDirectory, "*.json");
            foreach(string file in jsonFiles)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                DateTime creationTime = File.GetCreationTime(file);
                animationFiles.Add(new AnimationFile(name, creationTime));
            }
        } catch (Exception e)
        {
            Debug.LogError($"Failed to fetch files: {e.Message}");
        }

        return animationFiles.OrderByDescending(x => x.CreationTime).ToList();
    }
}
