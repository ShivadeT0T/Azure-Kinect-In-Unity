using UnityEngine;
using TMPro;

public class AnimationDetailsView : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Date;
    public AnimationDetails animationObj;
    public MenuUI uiScript;

    private void Start()
    {
        uiScript = GameObject.FindGameObjectWithTag("MenuUI").GetComponent<MenuUI>();
    }
    public void UpdateAnimationDetails(AnimationDetails animationDetails)
    {
        Name.text = animationDetails.Name;
        Date.text = animationDetails.Date;
        animationObj = animationDetails;
    }

    public void LoadButton()
    {
        uiScript.ShowDialog(animationObj, DialogType.LOAD);
    }

    public void DeleteButton()
    {
        uiScript.ShowDialog(animationObj, DialogType.DELETE);
    }
}