using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
public class MenuUI : MonoBehaviour
{
    public List<AnimationDetails> animationList;

    public GameObject scrollViewContent;
    public AnimationDetailsView animationDetailsPrefab;

    public GameObject LoadScreenCanvas;

    public GameObject DialogCanvas;
    public ConfirmationDialog DialogPrefab;

    public GameObject ErrorDialog;
    public TMP_Text errorMessage;

    public DialogType dialogType = DialogType.DEFAULT;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animationList = new List<AnimationDetails>();
        foreach (AnimationFile file in FileManager.LoadFilesInfo())
        {
            animationList.Add(new AnimationDetails(file.Name, file.CreationTime.ToString("yyyy-MM-dd HH:mm")));
        }

        switch (InfoBetweenScenes.menuState)
        {
            case MenuState.NORMAL:
                Debug.Log("Normal");
                break;
            case MenuState.ERROR:
                Debug.Log("Error");
                errorMessage.text = InfoBetweenScenes.ErrorMessage;
                ShowCanvas(ErrorDialog);
                InfoBetweenScenes.menuState = MenuState.NORMAL;
                break;
            default:
                Debug.Log("Default");
                break;
        }
    }
    public void CloseCanvas(GameObject canvas)
    {
        canvas.SetActive(false);
    }

    public void ShowCanvas(GameObject canvas)
    {
        canvas.SetActive(true);
    }

    public void ShowDialog(AnimationDetails animationObj, DialogType dialogType)
    {
        DialogPrefab.UpdateDialog(animationObj, dialogType);
        ShowCanvas(DialogCanvas);
    }

    public void LoadScene(string fileName)
    {
        InfoBetweenScenes.AnimationFileName = fileName;
        SceneManager.LoadScene("LoadRecording");
    }

    public void RecordScene()
    {
        SceneManager.LoadScene("SaveRecording");
    }

    public void DeleteFile(AnimationDetails animationObj)
    {
        //Debug.Log("Delete file function called. Work in progress...");
        FileManager.DeleteFile(animationObj.Name);
        animationList.Remove(animationObj);
        CloseCanvas(DialogCanvas);
        CloseCanvas(LoadScreenCanvas);
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
        ShowCanvas(LoadScreenCanvas);
        
        foreach (AnimationDetails animationDetails in animationList)
        {
            AnimationDetailsView animDetailObj = Instantiate(animationDetailsPrefab) as AnimationDetailsView;
            animDetailObj.gameObject.SetActive(true);
            animDetailObj.UpdateAnimationDetails(animationDetails);
            animDetailObj.transform.SetParent(scrollViewContent.transform, false);
        }
    }
}
