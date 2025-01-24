using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum DialogType
{
    GAME,
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

public class ModelDetails
{
    public string Name;

    public ModelDetails(string name)
    {
        Name = name;
    }
}
public class MenuUI : MonoBehaviour
{

    //Model canvas and its related objects
    public List<ModelDetails> modelList;
    public ToggleGroup toggleGroup;
    public ModelDetailsView modelDetailsPrefab;
    public GameObject modelScrollViewContent;
    public GameObject modelScreenCanvas;


    public List<AnimationDetails> animationList;

    public GameObject animationScrollViewContent;
    public AnimationDetailsView animationDetailsPrefab;

    public GameObject practiceScrollViewContent;
    public AnimationDetailsView practiceDetailsPrefab;
    public GameObject PracticeScreenCanvas;

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

        modelList = new List<ModelDetails>();
        foreach (var model in Resources.LoadAll("PrefabModels", typeof(GameObject)))
        {
            modelList.Add(new ModelDetails(model.name));
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

    public void GameScene(string fileName)
    {
        InfoBetweenScenes.AnimationFileName = fileName;
        SceneManager.LoadScene("GameScene");
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

    public void ClearGamePrefab()
    {
        foreach (Transform transf in practiceScrollViewContent.transform)
        {
            Destroy(transf.gameObject);
        }
    }

    public void ClearAnimationPrefab()
    {
        foreach (Transform transf in animationScrollViewContent.transform)
        {
            Destroy(transf.gameObject);
        }
    }

    public void ClearModelPrefab()
    {
        foreach (Transform transf in modelScrollViewContent.transform)
        {
            Destroy(transf.gameObject);
        }
    }

    public void DisplayGameDetailsList()
    {

        ClearGamePrefab();
        ShowCanvas(PracticeScreenCanvas);

        foreach (AnimationDetails animationDetails in animationList)
        {
            AnimationDetailsView animDetailObj = Instantiate(practiceDetailsPrefab) as AnimationDetailsView;
            animDetailObj.gameObject.SetActive(true);
            animDetailObj.UpdateAnimationDetails(animationDetails);
            animDetailObj.transform.SetParent(practiceScrollViewContent.transform, false);
        }
    }

    public void DisplayAnimationDetailsList()
    {

        ClearAnimationPrefab();
        ShowCanvas(LoadScreenCanvas);
        
        foreach (AnimationDetails animationDetails in animationList)
        {
            AnimationDetailsView animDetailObj = Instantiate(animationDetailsPrefab) as AnimationDetailsView;
            animDetailObj.gameObject.SetActive(true);
            animDetailObj.UpdateAnimationDetails(animationDetails);
            animDetailObj.transform.SetParent(animationScrollViewContent.transform, false);
        }
    }

    public void DisplayModelDetailsList()
    {
        ClearModelPrefab();
        ShowCanvas(modelScreenCanvas);

        foreach(ModelDetails modelDetails in modelList)
        {
            ModelDetailsView modelDetailObj = Instantiate(modelDetailsPrefab) as ModelDetailsView;
            toggleGroup.RegisterToggle(modelDetailObj.selfToggle);
            modelDetailObj.gameObject.SetActive(true);
            modelDetailObj.UpdateModelDetails(modelDetails);
            modelDetailObj.transform.SetParent(modelScrollViewContent.transform, false);

            if (modelDetailObj.Name.text == InfoBetweenScenes.prefabModelName) modelDetailObj.selfToggle.isOn = true;
        }
    }

    public void ConfirmModel()
    {
        Toggle selectedToggle = toggleGroup.ActiveToggles().FirstOrDefault();

        InfoBetweenScenes.prefabModelName = selectedToggle.GetComponent<ModelDetailsView>().Name.text;
        CloseCanvas(modelScreenCanvas);
    }


}
