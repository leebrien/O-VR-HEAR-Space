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
    private Coroutine _tutorialCoroutine;

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
            if (welcomeScreenLayout != null)
            {
                var titleObj = welcomeScreenLayout.transform.Find("welcomeInstructionTitle");
                if (titleObj)
                    _welcomeText = titleObj.GetComponent<TextMeshProUGUI>();
            }
            StartTutorialCoroutine(ShowTutorialPanelWithDelay());
        }
    }

    private void OnDisable()
    {
        StopTutorialCoroutine();
    }

    private Coroutine StartTutorialCoroutine(IEnumerator routine)
    {
        if (_tutorialCoroutine != null)
        {
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
        }

        _tutorialCoroutine = StartCoroutine(routine);
        return _tutorialCoroutine;
    }

    private void StopTutorialCoroutine()
    {
        if (_tutorialCoroutine != null)
        {
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
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

        timer = 8f;
        while (timer > 0f)
        {
            timerText.text = $"{Mathf.CeilToInt(timer)}s";
            yield return null;
            timer -= Time.deltaTime;
        }

        timerBody.SetActive(false);
        tutorialPanelGuide1.SetActive(false);
        CoreManager.Instance.SetFirstLog(false);
        practiceSoundManager?.GetTrackingSwitcher().SwitchToHandsOnly();

        // Mark this coroutine finished before calling the void method
        _tutorialCoroutine = null;

        // Call the void loader
        LoadNextTutorial();
    }

    public void LoadNextTutorial()
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
                practiceSoundManager?.GetTrackingSwitcher().SwitchToControllersOnly();
                tutorialPanelGuide3.SetActive(true);
                break;
            }

            case 3:
            {
                // long-wait step -> start dedicated coroutine
                StartTutorialCoroutine(TutorialStep3Routine());
                break;
            }

            case 4:
            {
                tutorialTitle.text = "Great job!";
                tutorialInstructions.text =
                    "Awesome! Now we can move on to learning more ways to interact in the VR world.";
                tutorialPanel.SetActive(true);
                tutorialReusableGuide.SetActive(true);
                nextButton.SetActive(true);
                break;
            }

            case 5:
            {
                tutorialReusableGuide.SetActive(false);
                tutorialPanelGuide4.SetActive(true);
                break;
            }

            case 6:
            {
                // long-wait step -> start dedicated coroutine
                StartTutorialCoroutine(TutorialStep6Routine());
                break;
            }

            case 7:
            {  practiceSoundManager?.GetTrackingSwitcher().SwitchToBoth();
                // long-wait step -> start dedicated coroutine
                StartTutorialCoroutine(TutorialStep7Routine());
                break;
            }
        }
    }

    private IEnumerator TutorialStep3Routine()
    {
        nextButton.SetActive(false);
        tutorialPanelGuide3.SetActive(false);
        practiceSoundManager?.GetTrackingSwitcher().SwitchToBoth();
        tutorialReusableGuide.SetActive(true);
        timerBody.SetActive(true);
        float waitTimer = 8f;
        while (waitTimer > 0f)
        {
            timerText.text = $"{Mathf.CeilToInt(waitTimer)}s";
            yield return null;
            waitTimer -= Time.deltaTime;
        }
        timerBody.SetActive(false);
        tutorialReusableGuide.SetActive(false);
        currentTaskType = 1;
        practiceSoundManager?.PlayObject(currentTaskType);
        tutorialPanel.SetActive(false);

        // mark coroutine finished
        _tutorialCoroutine = null;
    }

    private IEnumerator TutorialStep6Routine()
    {
        nextButton.SetActive(false);
        tutorialPanelGuide4.SetActive(false);
        tutorialTitle.text = "Let's try grabbing!";
        tutorialInstructions.text =
            "Once the timer ends, walk toward the blue sphere and grab it with your hand.";
        tutorialReusableGuide.SetActive(true);
        timerBody.SetActive(true);
        float waitTimer6 = 8f;
        while (waitTimer6 > 0f)
        {
            timerText.text = $"{Mathf.CeilToInt(waitTimer6)}s";
            yield return null;
            waitTimer6 -= Time.deltaTime;
        }
        timerBody.SetActive(false);
        tutorialReusableGuide.SetActive(false);
        tutorialPanel.SetActive(false);
        currentTaskType = 2;
        practiceSoundManager?.PlayObject(currentTaskType);

        _tutorialCoroutine = null;
    }

    private IEnumerator TutorialStep7Routine()
    {
        
        tutorialTitle.text = "Nicely done!";
        tutorialInstructions.text =
            "You’ve completed the tutorial. Before proceeding we need you to answer a simulation sickness test first. " +
            "You'll be redirected in a moment.";
        tutorialPanel.SetActive(true);
        tutorialReusableGuide.SetActive(true);
        timerBody.SetActive(true);
        float waitTimer7 = 10f;
        while (waitTimer7 > 0f)
        {
            timerText.text = $"{Mathf.CeilToInt(waitTimer7)}s";
            yield return null;
            waitTimer7 -= Time.deltaTime;
        }

        // navigate away
        SceneManager.LoadScene("SSQ-Scene");

        _tutorialCoroutine = null;
    }

    public void OnHomeButtonClick()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void OnTaskComplete()
    {
        practiceSoundManager?.StopObject();

        tutorialPanel.SetActive(true);

        // advance synchronously (helper coroutines will run where needed)
        LoadNextTutorial();
    }
}
