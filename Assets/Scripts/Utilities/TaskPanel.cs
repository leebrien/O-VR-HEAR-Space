using UnityEngine;
using System.Collections;
using System.Globalization;
using TMPro;

public class TaskPanel : MonoBehaviour
{
    public GameObject startButton;
    public TextMeshProUGUI displayText;
    public TextMeshProUGUI counterTitleHolder;
    public TextMeshProUGUI counterTextHolder;
    public GameObject panelTextsHolder;
    public TextMeshProUGUI interactionStatusText;
    public GameObject panelToHide;
    
    public AudioSource ambientAudioSource;

    private const float TimerDuration = 5f;
    private TrackingSwitcher _trackingSwitcher;
    private SaberManager _saberManager;
    
    private int _currentTask = 0;

    private void Start()
    {

        string currentCondition = CoreManager.Instance.currentCondition;
         _currentTask = CoreManager.Instance.currentTask;
        if (interactionStatusText != null) interactionStatusText.gameObject.SetActive(false);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Task"))
        {
            var taskType = _currentTask switch
            {
                1 => "Pointing",
                2 => "Grabbing",
                3 => "Complex Grabbing",
                _ => "Unknown Task"
            };

            var cueType = currentCondition switch
            {
                "PC" => "Personalized Cue",
                "GC" => "Generic Cue",
                _ => "Unknown Cue"
            };

            displayText.text = $"TASK {_currentTask}: {taskType} ({cueType})";
        }
        if (panelTextsHolder != null) panelTextsHolder.SetActive(true);
    }

    public void OnStartButtonClicked()
    {
        if (startButton != null)
        {
            panelTextsHolder.SetActive(false);
            displayText.gameObject.SetActive(false);
            startButton.SetActive(false);
            StartCoroutine(CountdownTimer());
        }
    }

    public void ShowPanel()
    {
        if (panelToHide != null)
        {
            displayText.gameObject.SetActive(true);
            panelToHide.SetActive(true);
        }
    }

    public void OnProceedClick()
    {
        if (interactionStatusText) interactionStatusText.gameObject.SetActive(false);
        CoreManager.Instance.LoadNextScene();
    }

    // Panel countdown
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator CountdownTimer()
    {
        float currentTime = TimerDuration;

        if (counterTextHolder!=null)
        {
            counterTitleHolder.gameObject.SetActive(true);
            displayText.gameObject.SetActive(false);
        }

        while (currentTime > 0)
        {
            counterTextHolder.text = Mathf.Ceil(currentTime).ToString(CultureInfo.InvariantCulture);
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        counterTextHolder.text = "0";
        if (CoreManager.Instance.currentTask == 1)
        {
            _trackingSwitcher = FindFirstObjectByType<TrackingSwitcher>();
            _saberManager = FindFirstObjectByType<SaberManager>();

            if (_trackingSwitcher != null && _saberManager != null)
            {
                _trackingSwitcher.SwitchToControllersOnly();
                _saberManager.EnableSabers();
            }
        }
        else
        {
            _trackingSwitcher = FindFirstObjectByType<TrackingSwitcher>();
            if (_trackingSwitcher != null) _trackingSwitcher.SwitchToHandsOnly();
        }

        if (_currentTask == 3)
        {
            ambientAudioSource.Play();
        }
        
        yield return new WaitForSeconds(1f);

        if (displayText!=null)
        {
            counterTitleHolder.gameObject.SetActive(false);
            counterTextHolder.gameObject.SetActive(false);
        }
        if (panelToHide!=null)
        {
            panelToHide.SetActive(false);

            if (SoundManager.Instance)
            {
                SoundManager.Instance.PlaySound();
                CoreManager.Instance.StartCurrentTaskTime();
            }
        }
    }
}
