using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DialogType
{
    LOAD,
    DELETE,
    DEFAULT
}
public class AnimationDetails
{
    public string Name;
    public string Date;

    public AnimationDetails(string name, string date)
    {
        Name = name;
        Date = date;
    }
}
public class GUI : MonoBehaviour
{
    public List<AnimationDetails> animationList;

    public GameObject scrollViewContent;
    public AnimationDetailsView animationDetailsPrefab;

    public GameObject LoadScreenCanvas;
    public ConfirmationDialog DialogCanvas;

    public DialogType dialogType = DialogType.DEFAULT;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animationList = new List<AnimationDetails>();
        foreach (AnimationFile file in FileManager.LoadFilesInfo())
        {
            animationList.Add(new AnimationDetails(file.Name, file.CreationTime.ToString("yyyy-MM-dd HH:mm")));
        }
    }
    public void CloseCanvas(GameObject canvas)
    {
        canvas.SetActive(false);
    }

    public void ShowDialog(string name, HandlerType type)
    {

    }

    public void ShowLoadScreen()
    {
        LoadScreenCanvas.SetActive(true);
    }

    public void ClearPrefab()
    {
        foreach (Transform transf in scrollViewContent.transform)
        {
            Destroy(transf.gameObject);
        }
    }

    public void DisplayAnimationDetailsList()
    {

        ClearPrefab();
        ShowLoadScreen();
        
        foreach (AnimationDetails animationDetails in animationList)
        {
            AnimationDetailsView animDetailObj = Instantiate(animationDetailsPrefab) as AnimationDetailsView;
            animDetailObj.gameObject.SetActive(true);
            animDetailObj.UpdateAnimationDetails(animationDetails);
            animDetailObj.transform.SetParent(scrollViewContent.transform, false);
        }
    }

}
