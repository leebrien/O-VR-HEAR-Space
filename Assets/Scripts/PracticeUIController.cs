using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PracticeUIManagement : MonoBehaviour
{
    public GameObject welcomeScreenLayout;
    public GameObject tutorialPanelGuide1;
    public GameObject tutorialPanelGuide2;
    public GameObject tutorialPanelGuide3;
    public GameObject tutorialPanelGuide4;
    public GameObject tutorialReusableGuide;
    public GameObject timerBody;
    public TextMeshProUGUI timerText;
    public GameObject nextButton;
    public TextMeshProUGUI tutorialTitle;
    public TextMeshProUGUI tutorialInstructions;
    public GameObject tutorialPanel;

    private bool _firstLog;
    private int currentTaskType = 0;

    public PracticeSoundManager practiceSoundManager;
    private int _tutorialIndex = 0;

    private TextMeshProUGUI _welcomeText;

    private void Start()
    {
        _firstLog = practiceSoundManager.GetLoggingStatus();

        if (!_firstLog)
        {
            _tutorialIndex = 0;
            tutorialPanelGuide1.SetActive(true);
        }
        else
        {
            if (welcomeScreenLayout!=null)
            {
                var titleObj = welcomeScreenLayout.transform.Find("welcomeInstructionTitle");
                if (titleObj)
                    _welcomeText = titleObj.GetComponent<TextMeshProUGUI>();
            }
            StartCoroutine(ShowTutorialPanelWithDelay());
        }
    }

    private IEnumerator ShowTutorialPanelWithDelay()
    {
        float timeDelay = 6f;
        float timer = timeDelay;
        while (timer > 0f)
        {
            if (_welcomeText)
                _welcomeText.text = $"The VR environment tutorial will begin shortly in {Mathf.CeilToInt(timer)}s.";
            yield return null;
            timer -= Time.deltaTime;
        }
        welcomeScreenLayout.SetActive(false);
        tutorialPanelGuide1.SetActive(true);
        timerBody.SetActive(true);
        timer = 10f;
        while (timer > 0f)
        {
            timerText.text = $"{Mathf.CeilToInt(timer)}s";
            yield return null;
            timer -= Time.deltaTime;
        }
        timerBody.SetActive(false);
        tutorialPanelGuide1.SetActive(false);
        CoreManager.Instance.SetFirstLog(false);
        practiceSoundManager.GetTrackingSwitcher().SwitchToHandsOnly();
    }


    public IEnumerator LoadNextTutorial()
    {
        _tutorialIndex++;

        switch (_tutorialIndex)
        {
            case 1:
            {
                tutorialPanelGuide2.SetActive(true);
                nextButton.SetActive(true);
                break;
            }

            case 2:
            {
                tutorialPanelGuide2.SetActive(false);
                practiceSoundManager.GetTrackingSwitcher().SwitchToControllersOnly();
                tutorialPanelGuide3.SetActive(true);
                break;
            }

            case 3:
            {
                nextButton.SetActive(false);
                tutorialPanelGuide3.SetActive(false);
                practiceSoundManager.GetTrackingSwitcher().SwitchToBoth();
                tutorialReusableGuide.SetActive(true);
                timerBody.SetActive(true);
                var timer = 8f;
                while (timer > 0f)
                {
                    timerText.text = $"{Mathf.CeilToInt(timer)}s";
                    yield return null;
                    timer -= Time.deltaTime;
                }
                timerBody.SetActive(false);
                tutorialReusableGuide.SetActive(false);
                currentTaskType = 1;
                if (practiceSoundManager != null)
                {
                    practiceSoundManager.PlayObject(currentTaskType);
                }
                break;
            }

            case 4:
            {
                tutorialTitle.text = "Great job!";
                tutorialInstructions.text =
                    "Awesome! Now we can move on to learning more ways to interact in the VR world.";
                tutorialReusableGuide.SetActive(true);
                nextButton.SetActive(true);
                break;
            }

            case 5:
            {
                tutorialPanelGuide4.SetActive(true);
                break;
            }

            case 6:
            {
                nextButton.SetActive(false);
                tutorialPanelGuide4.SetActive(false);
                tutorialTitle.text = "Let's try grabbing!";
                tutorialInstructions.text =
                    "Once the timer ends, walk toward the blue sphere and grab it with your hand.";
                tutorialReusableGuide.SetActive(true);
                timerBody.SetActive(true);
                var timer = 8f;
                while (timer > 0f)
                {
                    timerText.text = $"{Mathf.CeilToInt(timer)}s";
                    yield return null;
                    timer -= Time.deltaTime;
                }
                timerBody.SetActive(false);
                tutorialReusableGuide.SetActive(false);
                currentTaskType = 2;
                if (practiceSoundManager != null)
                {
                    practiceSoundManager.PlayObject(currentTaskType);
                }
                break;
            }

            case 7:
            {
                tutorialTitle.text = "Nicely done!";
                tutorialInstructions.text =
                    "You’ve completed the tutorial. Before proceeding we need you to answer a simulation sickness test first. " +
                    "You'll be redirected in a moment.";
                tutorialReusableGuide.SetActive(true);
                timerBody.SetActive(true);
                var timer = 10f;
                while (timer > 0f)
                {
                    timerText.text = $"{Mathf.CeilToInt(timer)}s";
                    yield return null;
                    timer -= Time.deltaTime;
                }
                // SceneManager.LoadScene("SSQ_Scene");
                
                /*tutorialTitle.text = "Thank you!";
                tutorialInstructions.text =
                    "You'll now be redirected to the lobby. " +
                    "You’ll see a panel with three buttons: Play Tutorial, Import from Hearsona, and Proceed.";
                tutorialReusableGuide.SetActive(true);
                timerBody.SetActive(true);
                var timer = 10f;
                while (timer > 0f)
                {
                    timerText.text = $"{Mathf.CeilToInt(timer)}s";
                    yield return null;
                    timer -= Time.deltaTime;
                }*/
                SceneManager.LoadScene("LobbyScene");
                break;
            }
        }
    }


    public void OnHomeButtonClick()
    {
        SceneManager.LoadScene("LobbyScene");
    }


    public void OnStartButtonClick()
    {
        // Hide the main lobby panel to start the task
        tutorialPanel.SetActive(false);

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

        tutorialPanel.SetActive(true);

        StartCoroutine(LoadNextTutorial());
    }

}
