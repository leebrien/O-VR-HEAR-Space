using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TaskPanel : MonoBehaviour
{
    public GameObject startButton;
    public TextMeshProUGUI displayText;
    public GameObject panelToHide;
    public SoundManager soundManager;
    public AudioSource ambientNoise;

    private const float timerDuration = 5f;

    private void Awake()
    {
        string currentCondition = CoreManager.Instance.currentCondition;
        int currentTask = CoreManager.Instance.currentTask;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Task"))
        {
            displayText.text = $"Task {currentTask} ({currentCondition})";
        }
    }

    public void OnStartButtonClicked()
    {
        if (startButton != null)
        {
            startButton.SetActive(false);
            StartCoroutine(CountdownTimer());
        }
    }

    public void ShowPanel()
    {
        if (panelToHide != null)
        {
            panelToHide.SetActive(true);
        }
    }

    public void OnProceedClick()
    {
        CoreManager.Instance.LoadNextScene();
    }

    // Panel countdown
    private IEnumerator CountdownTimer()
    {
        float currentTime = timerDuration;

        if (displayText != null)
        {
            displayText.gameObject.SetActive(true);

            RectTransform textTransform = displayText.GetComponent<RectTransform>();
            if (textTransform != null)
            {
                Vector2 newPos = textTransform.anchoredPosition;
                newPos.y -= 10;
                textTransform.anchoredPosition = newPos;
            }
            displayText.fontSize = 30;
        }

        while (currentTime > 0)
        {
            displayText.text = Mathf.Ceil(currentTime).ToString();
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        displayText.text = "0";
        yield return new WaitForSeconds(1f);

        if (ambientNoise != null)
        {
            ambientNoise.Play();
        }

        if (displayText != null)
        {
            displayText.gameObject.SetActive(false);
        }

        if (panelToHide != null)
        {
            panelToHide.SetActive(false);

            if (soundManager != null)
            {
                soundManager.PlaySound();
            }
        }

    }
}
