
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using UnityEngine;

public class FileManager
{
    public static string animationDirectory = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Animations";
    public static void CreateJsonFile(string fileName, string json)
    {
        try
        {
            string jsonFile = fileName + ".json";

            string filePath = Path.Combine(animationDirectory, jsonFile);
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
            string jsonFile = fileName + ".json";

            string filePath = Path.Combine(animationDirectory, jsonFile);
            animationJson = File.ReadAllText(filePath);
            Debug.Log("Loading JSON file successful");

        } catch (Exception e)
        {
            Debug.LogError($"Couldn't load JSON file: {e.Message}");
        }
            return animationJson;
    }
    public static void DeleteFile(string fileName)
    {
        try
        {
            string jsonFile = fileName + ".json";
            string metaFile = fileName + ".json.meta";

            string jsonFilePath = Path.Combine(animationDirectory, jsonFile);
            string metaFilePath = Path.Combine(animationDirectory, metaFile);
            File.Delete(jsonFilePath);
            File.Delete(metaFilePath);
            Debug.Log("JSON file deleted successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed deleting a file: {e.Message}");
        }
    }

    public static IList<AnimationFile> LoadFilesInfo()
    {
        List<AnimationFile> animationFiles = new List<AnimationFile>();
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
