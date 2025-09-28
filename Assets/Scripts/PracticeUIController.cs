using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PracticeUIManagement : MonoBehaviour
{
    public GameObject taskSelectionLayout;
    public GameObject taskPrepLayout;
    public GameObject lobbyPanel;
    public TextMeshProUGUI taskGoalText;

    private int currentTaskType = 0;

    public PracticeSoundManager practiceSoundManager;

    private void Start()
    {
        taskSelectionLayout.SetActive(true);
        taskPrepLayout.SetActive(false);
    }
    

    public void OnHomeButtonClick()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    
    public void OnPointingButtonClick()
    {
        taskSelectionLayout.SetActive(false);
        taskPrepLayout.SetActive(true);
        taskGoalText.text = "Goal: Point at the blue sphere.";
        currentTaskType = 1;
    }

    public void OnGrabbingButtonClick()
    {
        taskSelectionLayout.SetActive(false);
        taskPrepLayout.SetActive(true);
        taskGoalText.text = "Goal: Approach and grab the blue sphere.";
        currentTaskType = 2;
    }

    public void OnReturnButtonClick()
    {
        taskPrepLayout.SetActive(false);
        taskSelectionLayout.SetActive(true);
        taskGoalText.text = "";
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
    public void OnTaskComplete()
    {
        // Stop the object and show the lobby panel again
        if (practiceSoundManager != null)
        {
            practiceSoundManager.StopObject();
        }
        lobbyPanel.SetActive(true);
        taskPrepLayout.SetActive(false);
        taskSelectionLayout.SetActive(true);
        taskGoalText.text = "";
    }
}