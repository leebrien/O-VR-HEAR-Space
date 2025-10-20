using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PracticeUIManagement : MonoBehaviour
{
    public GameObject lobbyPanel;
    public GameObject welcomeScreenLayout;
    public GameObject imageContainer;
    public GameObject tutorialPanelGuide;
    private bool _firstLog;
    private int currentTaskType = 0;

    public PracticeSoundManager practiceSoundManager;
    private int _tutorialIndex = 0;

    private TextMeshProUGUI _instructionTutorial;
    private TextMeshProUGUI _welcomeText;

    private void Start()
    {
        _firstLog = practiceSoundManager.GetLoggingStatus();
        
        if (!_firstLog)
        {
            _tutorialIndex = 0;
            tutorialPanelGuide.SetActive(true);
        }
        
        if (tutorialPanelGuide != null)
        {
            var titleObj = tutorialPanelGuide.transform.Find("tutorialInstructionTitle");
            if (titleObj)
                _instructionTutorial = titleObj.GetComponent<TextMeshProUGUI>();
        }

        if (welcomeScreenLayout!=null)
        {
            var titleObj = welcomeScreenLayout.transform.Find("welcomeInstructionTitle");
            if (titleObj)
                _welcomeText = titleObj.GetComponent<TextMeshProUGUI>();
            
        }
        
        StartCoroutine(ShowTutorialPanelWithDelay());
    }
    
    private IEnumerator ShowTutorialPanelWithDelay()
    {
        float timeDelay = 5f;
        float timer = timeDelay;
        while (timer > 0f)
        {
            if (_welcomeText)
                _welcomeText.text = $"The VR environment tutorial will begin shortly in {Mathf.CeilToInt(timer)} seconds.";
            yield return null;
            timer -= Time.deltaTime;
        }
        welcomeScreenLayout.SetActive(false);
        tutorialPanelGuide.SetActive(true);
        CoreManager.Instance.SetFirstLog(false);
    }
    

    public void LoadNextTutorial()
    {
        if (_tutorialIndex == 1) imageContainer.SetActive(false);
        
        _tutorialIndex++;

        switch (_tutorialIndex)
        {
            case 1:
                practiceSoundManager.GetTrackingSwitcher().SwitchToControllersOnly();
                _instructionTutorial.text =
                    "You can also interact with objects from a distance using the controller." +
                    "\nSimply point your hand toward the target and press the trigger with your index finger. " +
                    "Below is the visual reference showing how to press the trigger.";
                imageContainer.SetActive(true);
                break;
            
            case 2:
                _instructionTutorial.text =
                    "You’ll now try pointing using the same method as selecting." +
                    "\nAfter pressing Next, aim the line at the blue sphere and press the trigger to point.";
                break;
            
            case  3:
                currentTaskType = 1;
                lobbyPanel.SetActive(false);
                if (practiceSoundManager != null)
                {
                    practiceSoundManager.PlayObject(currentTaskType);
                }
                break;
            
            case 4:
                practiceSoundManager.GetTrackingSwitcher().SwitchToHandsOnly();
                _instructionTutorial.text =
                    "Now that you know how to point, let's try grabbing." +
                    "\nAfter pressing Next, walk toward the blue sphere and grab it using your hand.";
                break;
            case 5:
                currentTaskType = 2;
                lobbyPanel.SetActive(false);
                if (practiceSoundManager != null)
                {
                    practiceSoundManager.PlayObject(currentTaskType);
                }
                break;
            case 6:
                _instructionTutorial.text =
                    "You've completed the tutorial!" +
                    "\nAfter pressing Next, you'll be redirected to the main study lobby." +
                    "\nBefore you can proceed, make sure to import your Hearsona by clicking the 'Import Hearsona' button on the next screen.";
                break;
            case 7:
                SceneManager.LoadScene("LobbyScene");
                break;
        }
        
    }
    

    public void OnHomeButtonClick()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    

    public void OnStartButtonClick()
    {
        // Hide the main lobby panel to start the task
        lobbyPanel.SetActive(false);

        // Use the stored task type to generate the object
        if (practiceSoundManager != null)
        {
            practiceSoundManager.PlayObject(currentTaskType);
        }
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public void OnTaskComplete()
    {
        // Stop the object and show the lobby panel again
        practiceSoundManager?.StopObject();

        lobbyPanel.SetActive(true);
        
        LoadNextTutorial();
    }
}