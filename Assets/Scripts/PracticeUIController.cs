using System.Collections;
using TMPro;
using UnityEngine;

public class PracticeUIManagement : MonoBehaviour
{
    public GameObject tutorialPanelGuide1;
    public GameObject tutorialPanelGuide2;
    public GameObject tutorialPanelGuide3;
    public GameObject tutorialPanelGuide4;
    public GameObject tutorialReusableGuide;
    public GameObject protocolSequencePanel;
    public GameObject timerBody;
    public TextMeshProUGUI timerText;
    public GameObject nextButton;
    public TextMeshProUGUI tutorialTitle;
    public TextMeshProUGUI tutorialInstructions;
    public GameObject tutorialPanel;

    private bool _firstLog;
    private int _currentTaskType;

    public PracticeSoundManager practiceSoundManager;
    private int _tutorialIndex;

    private TextMeshProUGUI _welcomeText;
    private Coroutine _tutorialCoroutine;

    private void Start()
    {
        _firstLog = practiceSoundManager.GetLoggingStatus();
        Debug.Log(_firstLog);


        if (_firstLog)
        {
            Debug.LogWarning("PracticeUIManagement: My first log - "  + _firstLog);
            _tutorialIndex = 0;
            _tutorialIndex = 0;
            StartTutorialCoroutine(InitialTutorial());
        }
        else
        {
            tutorialPanelGuide1.SetActive(false);
            protocolSequencePanel.SetActive(true);
            practiceSoundManager?.GetTrackingSwitcher().SwitchToBoth();
        }
    }

    private IEnumerator InitialTutorial()
    {
        timerBody.SetActive(true);
        float timeDelay = 8f;
        float timer = timeDelay;
        while (timer > 0f)
        {
            timerText.text = $"{Mathf.CeilToInt(timer)}s";
            yield return null;
            timer -= Time.deltaTime;
        }
        tutorialPanelGuide1.SetActive(false);
        timerBody.SetActive(false);
        CoreManager.Instance.SetFirstLog(false);
        practiceSoundManager?.GetTrackingSwitcher().SwitchToHandsOnly();
        _tutorialCoroutine = null;
        LoadNextTutorial();
        
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
                tutorialPanelGuide3.SetActive(true);
                practiceSoundManager?.GetTrackingSwitcher().SwitchToControllersOnly();
                break;
            }

            case 3:
            {
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
                practiceSoundManager?.GetTrackingSwitcher().SwitchToBoth();
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
        _currentTaskType = 1;
        practiceSoundManager?.PlayObject(_currentTaskType);
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
        practiceSoundManager?.GetTrackingSwitcher().SwitchToHandsOnly();
        tutorialReusableGuide.SetActive(false);
        tutorialPanel.SetActive(false);
        _currentTaskType = 2;
        practiceSoundManager?.PlayObject(_currentTaskType);

        _tutorialCoroutine = null;
    }

    private IEnumerator TutorialStep7Routine()
    {
        
        tutorialTitle.text = "Nicely done!";
        tutorialInstructions.text =
            "You’ve completed the tutorial. You need to select the SET assigned to you. " +
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

        _tutorialCoroutine = null;
        // navigate away
        //SceneLoader.LoadScene("LobbyScene");
        
        timerBody.SetActive(false);
        tutorialReusableGuide.SetActive(false);
        protocolSequencePanel.SetActive(true);
    }

    //public void OnHomeButtonClick()
    //{
     //   SceneLoader.LoadScene("LobbyScene");
    //}

    // ReSharper disable Unity.PerformanceAnalysis
    public void OnTaskComplete()
    {
        practiceSoundManager?.StopObject();

        tutorialPanel.SetActive(true);

        // advance synchronously (helper coroutines will run where needed)
        LoadNextTutorial();
    }

    public void SelectSet(string setID)
    {
        if (CoreManager.Instance != null)
        {
            CoreManager.Instance.GenerateProtolSceneSequence(setID);
        }
        else
        {
            Debug.LogError("CoreManager Instance not found! Did you start from Bootstrap?");
        }
    }
}
