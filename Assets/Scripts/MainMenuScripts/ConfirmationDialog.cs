using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class ConfirmationDialog : MonoBehaviour
{
    public TMP_Text message;
    public AnimationDetails m_animationObj;
    public MenuUI uiScript;
    public GameObject selfCanvas;
    public DialogType dType = DialogType.DEFAULT;

    public void UpdateDialog(AnimationDetails animationObj, DialogType dialogType)
    {
        m_animationObj = animationObj;
        dType = dialogType;
        switch (dialogType)
        {
            case DialogType.GAME:
                message.text = $"Practice \"{FormatString(animationObj.Name)}\" File?";
                break;
            case DialogType.LOAD:
                message.text = $"Load \"{FormatString(animationObj.Name)}\" File?";
                break;
            case DialogType.DELETE:
                message.text = $"Delete \"{FormatString(animationObj.Name)}\" File?";
                break;
            default:
                message.text = $"Are you sure?";
                Debug.Log("DialogType stays the same inside ConfirmationDialog");
                break;
        }
    }

    public void ConfirmFunction()
    {
        switch (dType)
        {
            case DialogType.GAME:
                uiScript.GameScene(m_animationObj.Name);
                break;
            case DialogType.LOAD:
                uiScript.LoadScene(m_animationObj.Name);
                break;
            case DialogType.DELETE:
                uiScript.DeleteFile(m_animationObj);
                break;
            default:
                uiScript.CloseCanvas(selfCanvas);
                break;
        }
    
    }
    public void CancelFunction()
    {
        uiScript.CloseCanvas(selfCanvas);
    }

    private string FormatString(string s)
    {
        int MaxLength = 5;
        if (s.Length > MaxLength)
        {
            s = s.Substring(0, MaxLength) + "...";
        }
        return s;
    }

}
