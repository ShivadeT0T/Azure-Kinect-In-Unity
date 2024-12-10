using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModelDetailsView : MonoBehaviour
{
    public TMP_Text Name;
    public Toggle selfToggle;
    public void UpdateModelDetails(ModelDetails modelDetails)
    {
        Name.text = modelDetails.Name;
    }
}
