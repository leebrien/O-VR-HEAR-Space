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
    public GameObject panel_texts_holder;
    public TextMeshProUGUI interactionStatusText;
    public GameObject panelToHide;

    private const float _timerDuration = 5f;

    private void Awake()
    {
        string currentCondition = CoreManager.Instance.currentCondition;
        int currentTask = CoreManager.Instance.currentTask;
        if (interactionStatusText) interactionStatusText.gameObject.SetActive(false);
        
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
        panel_texts_holder.SetActive(true);
    }

    public void OnStartButtonClicked()
    {
        if (startButton != null)
        {
            panel_texts_holder.SetActive(false);
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
    private IEnumerator CountdownTimer()
    {
        float currentTime = _timerDuration;

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
            }
        }
    }
}
