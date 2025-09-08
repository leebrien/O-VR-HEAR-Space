using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelUIController : MonoBehaviour
{
    public GameObject conditionUI;
    public GameObject optionsUI;
    public AudioSource audioSource;
    public TextMeshProUGUI displayText;


    private string selectedCondition;

    public void OnGenericButtonClick()
    {
        displayText.text = "Generic cue is ready.";
        selectedCondition = "GC";

        conditionUI.SetActive(false);
        optionsUI.SetActive(true);
    }

    public void OnPersonalizedButtonClick()
    {
        displayText.text = "Personalized button was clicked.";
        selectedCondition = "PC";

        /*
        conditionUI.SetActive(false);
        optionsUI.SetActive(true);
        */
    }

    public void OnProceedClick()
    {
        if (string.IsNullOrEmpty(selectedCondition))
        {
            Debug.LogWarning("No condition selected. Cannot proceed.");
            return;
        }

        CoreManager.Instance.SetStartingCondition(selectedCondition);
        CoreManager.Instance.LoadNextScene();
    }

    public void OnReturnClick()
    {
        displayText.text = "Choose the starting condition.";
        selectedCondition = null;

        conditionUI.SetActive(true);
        optionsUI.SetActive(false);
    }
}