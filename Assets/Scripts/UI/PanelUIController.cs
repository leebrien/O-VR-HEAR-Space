using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelUIController : MonoBehaviour
{
    public GameObject lobbyMenuDesign;
    public GameObject hearsonaCueDownloadDesign;
    public GameObject homeButton;
    public SoundPolling soundPolling;
    
    private string _selectedCondition;
    private Button _proceedButton;
    private bool _cancelPolling;
    
    
    private void Start()
    {

        _proceedButton = lobbyMenuDesign.transform.Find("rowButtonLayout/proceedButton").GetComponent<Button>();
        _proceedButton.interactable = false;
    }
    

    
    public void OnImportButtonClick()
    {
        var pollTitleTMP = hearsonaCueDownloadDesign.transform.Find("pollTitle").GetComponent<TextMeshProUGUI>();
        var hintTitleTMP = hearsonaCueDownloadDesign.transform.Find("hintTitle").GetComponent<TextMeshProUGUI>();
        var doneButtonPoll = hearsonaCueDownloadDesign.transform.Find("doneButton").GetComponent<Button>();
        
        _cancelPolling = false;
        _proceedButton.interactable = false;
        lobbyMenuDesign.SetActive(false);
        
        pollTitleTMP.text = "Retrieving personalized cue from Hearsona";
        hintTitleTMP.text = "Please wait momentarily as we prepare the sounds for you";
        
        hearsonaCueDownloadDesign.SetActive(true);
        hearsonaCueDownloadDesign.transform.Find("Spinner").gameObject.SetActive(true);
        
        StartCoroutine(soundPolling.PollAudio(()=> _cancelPolling, success =>
        {
            if (success)
            {
                _proceedButton.interactable = true;
                hearsonaCueDownloadDesign.transform.Find("Spinner").gameObject.SetActive(false);
                homeButton.SetActive(false);
                doneButtonPoll.gameObject.SetActive(true);
                pollTitleTMP.text = "Personalized cue retrieved successfully!";
                hintTitleTMP.text = "Your cue is now ready. You may now go back to the lobby and proceed.";
            }
            else
            {
                pollTitleTMP.text = "Failed to retrieve cue!";
                hintTitleTMP.text = "Please try again later.";
                homeButton.SetActive(true);
            }
        }));
    }
    
    

    public void OnProceedClick()
    {
        //CoreManager.Instance.SetStartingCondition(_selectedCondition);
        // Set the protocol sequence
        CoreManager.Instance.LoadNextScene();
    }

    public void OnHomeButtonClick()
    {
        homeButton.SetActive(false);
        hearsonaCueDownloadDesign.SetActive(false);
        lobbyMenuDesign.SetActive(true);
        
    }
    
}