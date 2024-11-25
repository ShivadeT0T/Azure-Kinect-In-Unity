using UnityEngine;
using TMPro;

public class AnimationDetailsView : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Date;
    public GUI guiScript;

    private void Start()
    {
        guiScript = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUI>();
    }
    public void UpdateAnimationDetails(AnimationDetails animationDetails)
    {
        Name.text = animationDetails.Name;
        Date.text = animationDetails.Date;
    }

    public void LoadButton()
    {
        
    }
}