using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ModelLoading : MonoBehaviour
{
    public GameObject tracker;
    public RecordingManager manager; 
    public Toggle showSkeleton;
    public float OffsetX;
    void Start()
    {
        string modelPath = InfoBetweenScenes.prefabDirectory + InfoBetweenScenes.prefabModelName;
        var prefabInstance = Instantiate(Resources.Load(modelPath)) as GameObject;
        if (prefabInstance)
        {
            prefabInstance.transform.SetParent(tracker.transform);
            prefabInstance.GetComponent<PuppetAvatarPrefab>().OffsetX = OffsetX;
            showSkeleton.interactable = true;

            if (manager)
            {
                manager.model = prefabInstance;
                prefabInstance.SetActive(false);
            }
        }
    }
}
