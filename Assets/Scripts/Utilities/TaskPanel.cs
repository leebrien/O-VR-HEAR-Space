using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

    private const float TimerDuration = 5f;
    private TrackingSwitcher _trackingSwitcher;

    private void Awake()
    {
       
        string currentCondition = CoreManager.Instance.currentCondition;
        int currentTask = CoreManager.Instance.currentTask;
        if (interactionStatusText) interactionStatusText.gameObject.SetActive(false);
        if (currentTask == 1)
        {
            _trackingSwitcher = FindFirstObjectByType<TrackingSwitcher>();
           
        }
        
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Task"))
        {
            var taskType = currentTask switch
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
                _  => "Unknown Cue"
            };
                
            displayText.text = $"TASK {currentTask}: {taskType} ({cueType})";

            
        }
        panelTextsHolder.SetActive(true);
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

        if (counterTextHolder)
        {
            counterTitleHolder.gameObject.SetActive(true);
            displayText.gameObject.SetActive(false);
        }

        while (currentTime > 0)
        {
            counterTextHolder.text = Mathf.Ceil(currentTime).ToString();
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        counterTextHolder.text = "0";
        if (CoreManager.Instance.currentTask == 1 && _trackingSwitcher != null)
        {
            _trackingSwitcher.SwitchToControllersOnly();
            //Make the raycast saber like here in this point in time and once it is on success in task interaction switch it back
            
        }
        yield return new WaitForSeconds(1f);

        if (displayText)
        {
            counterTitleHolder.gameObject.SetActive(false);
            counterTextHolder.gameObject.SetActive(false);
        }
        if (panelToHide)
        {
            panelToHide.SetActive(false);

            if (SoundManager.Instance )
            {
                SoundManager.Instance.PlaySound();
                CoreManager.Instance.StartCurrentTaskTime();
            }
        }
    }
}
