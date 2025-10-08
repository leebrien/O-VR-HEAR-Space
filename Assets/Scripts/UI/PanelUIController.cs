using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelUIController : MonoBehaviour
{
    public GameObject lobbyMenuDesign;
    public GameObject cueSelectDesign;
    public GameObject hearsonaCueDownloadDesign;
    public GameObject preparationDesign;
    public GameObject homeButton;
    public TextMeshProUGUI displayText;
    public SoundPolling soundPolling;
    
    private string _selectedCondition;
    private Button _proceedButton;
    private bool _cancellPolling = false;
    
    
    private void Start()
    {
        _proceedButton = lobbyMenuDesign.transform.Find("rowButtonLayout/proceedButton").GetComponent<Button>();
        _proceedButton.interactable = false;
    }

    public void OnPlayTutorialButtonClick()
    {
        SceneManager.LoadScene("PracticeScene");
    }

    public void OnCueSelectButtonClick()
    {
        homeButton.SetActive(true);
        lobbyMenuDesign.SetActive(false);
        cueSelectDesign.SetActive(true);
        displayText.gameObject.SetActive(true);
    }
    
    public void OnImportButtonClick()
    {
        
        var pollTitleTMP = hearsonaCueDownloadDesign.transform.Find("pollTitle").GetComponent<TextMeshProUGUI>();
        var hintTitleTMP = hearsonaCueDownloadDesign.transform.Find("hintTitle").GetComponent<TextMeshProUGUI>();
        
        _cancellPolling = false;
        _proceedButton.interactable = false;
        
        Debug.Log("1");
        
        homeButton.SetActive(true);
        displayText.gameObject.SetActive(false);
        lobbyMenuDesign.SetActive(false);
        
        pollTitleTMP.text = "Retrieving personalized cue from Hearsona";
        hintTitleTMP.text = "Please wait momentarily as we prepare the sounds for you";
        
        hearsonaCueDownloadDesign.SetActive(true);
        hearsonaCueDownloadDesign.transform.Find("Spinner").gameObject.SetActive(true);
        Debug.Log("2");
        
        StartCoroutine(soundPolling.PollAudio(()=> _cancellPolling, success =>
        {
            if (success)
            {
                _proceedButton.interactable = true;
                hearsonaCueDownloadDesign.transform.Find("Spinner").gameObject.SetActive(false);
                pollTitleTMP.text = "Personalized cue retrieved successfully!";
                hintTitleTMP.text = "Your cue is now ready. You may now go back to the lobby and proceed.";
            }
            else
            {
                pollTitleTMP.text = "Failed to retrieve cue!";
                hintTitleTMP.text = "Please try again later.";
            }
        }));
    }

    public void OnGenericButtonClick()
    {
        displayText.text = "Selected starting cue: Generic.";
        _selectedCondition = "GC";
        cueSelectDesign.SetActive(false);
        preparationDesign.SetActive(true);
        displayText.gameObject.SetActive(true);
        
    }

    public void OnPersonalizedButtonClick()
    {
        displayText.text = "Selected starting cue: Personalized.";
        _selectedCondition = "PC";

        cueSelectDesign.SetActive(false);
        preparationDesign.SetActive(true);
        displayText.gameObject.SetActive(true);
    }

    public void OnProceedClick()
    {
        if (string.IsNullOrEmpty(_selectedCondition))
        {
            Debug.LogWarning("No condition selected. Cannot proceed.");
            return;
        }

        CoreManager.Instance.SetStartingCondition(_selectedCondition);
        CoreManager.Instance.LoadNextScene();
    }
    

    public void OnReturnClick()
    {
        displayText.text = "Please select the starting type of cue randomly assigned to you by the researchers.";
        _selectedCondition = null; 
        preparationDesign.SetActive(false);
        cueSelectDesign.SetActive(true);
    }
    
    public void OnHomeButtonClick()
    {
        homeButton.SetActive(false);
        if (preparationDesign.activeInHierarchy) preparationDesign.SetActive(false);
        if (cueSelectDesign.activeInHierarchy) cueSelectDesign.SetActive(false);
        if (hearsonaCueDownloadDesign.activeInHierarchy) hearsonaCueDownloadDesign.SetActive(false);
        //displayText.gameObject.SetActive(false);
        lobbyMenuDesign.SetActive(true);
    }
}