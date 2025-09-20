using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PanelUIController : MonoBehaviour
{
    public GameObject LobbyMenuDesign;
    public GameObject CueSelectDesign;
    public GameObject HearsonaCueDownloadDesign;
    public GameObject preparationDesign;
    public TextMeshProUGUI displayText;
    
    private string selectedCondition;
    private Button proceedButton;

    private void Start()
    {
        proceedButton = LobbyMenuDesign.transform.Find("rowButtonLayout/proceedButton").GetComponent<Button>();
        // proceedButton.interactable = false;
        proceedButton.interactable = false;
    }

    public void onPlayTutorialButtonClick()
    {
        SceneManager.LoadScene("PracticeScene");
    }

    public void onCueSelectButtonClick()
    {
        LobbyMenuDesign.SetActive(false);
        CueSelectDesign.SetActive(true);
        displayText.gameObject.SetActive(true);
    }
    
    public void onImportButtonClick()
    {
        displayText.gameObject.SetActive(false);
        LobbyMenuDesign.SetActive(false);
        HearsonaCueDownloadDesign.SetActive(true);
        
        StartCoroutine(WaitHearsona());
    }

    public void OnGenericButtonClick()
    {
        displayText.text = "Selected starting cue: Generic.";
        selectedCondition = "GC";
        CueSelectDesign.SetActive(false);
        preparationDesign.SetActive(true);
        displayText.gameObject.SetActive(true);
        
    }

    public void OnPersonalizedButtonClick()
    {
        displayText.text = "Selected starting cue: Personalized.";
        selectedCondition = "PC";

        CueSelectDesign.SetActive(false);
        preparationDesign.SetActive(true);
        displayText.gameObject.SetActive(true);
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
        displayText.text = "Please select the starting type of cue randomly assigned to you by the researchers.";
        selectedCondition = null; 
        preparationDesign.SetActive(false);
        CueSelectDesign.SetActive(true);
    }
    
    private IEnumerator WaitHearsona()
    {
        // pretend loading is happening
        //yield return new WaitForSeconds(4f);
        
        SoundManager.Instance.GetComponent<SoundPolling>().FetchAudioOnce();

        while (SoundManager.Instance.hearsonaCue == null)
        {
            yield return null;
        }

        // after "loading", switch panels back
        HearsonaCueDownloadDesign.SetActive(false);
        LobbyMenuDesign.SetActive(true);

        // enable the proceed button
        proceedButton.interactable = true;
    }
}