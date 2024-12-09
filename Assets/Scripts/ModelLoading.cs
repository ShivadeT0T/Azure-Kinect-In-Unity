using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ModelLoading : MonoBehaviour
{
    public GameObject tracker;
    public Toggle showSkeleton;
    void Start()
    {
        string modelPath = InfoBetweenScenes.prefabDirectory + InfoBetweenScenes.prefabModelName;
        var prefabInstance = Instantiate(Resources.Load(modelPath)) as GameObject;
        if (prefabInstance)
        {
            prefabInstance.transform.SetParent(tracker.transform);
            showSkeleton.interactable = true;
        }
    }
}
