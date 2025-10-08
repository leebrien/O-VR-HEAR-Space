using UnityEngine;
using TMPro;

public class TaskInteraction : MonoBehaviour
{
    public Material successMaterial;
    public TaskPanel taskPanel;
    public GameObject[] environmentObjects;
    public GameObject audioObject;
    public GameObject proceedButton;
    public TextMeshProUGUI displayText;
    public TextMeshProUGUI interactionStatusText;
    public Rigidbody audioRb;

    private Vector3 _soundPosition;
    private TrackingSwitcher _trackingSwitcher;

    // Flag to ensure OnSuccess runs only once per task completion
    private bool _isTaskCompleted = false;

    private void Awake()
    {
        if (CoreManager.Instance.currentTask == 1)
        {
            _trackingSwitcher = FindFirstObjectByType<TrackingSwitcher>();
        }

        _isTaskCompleted = false;
    }

    public void OnSuccess()
    {
        // Guard clause
        if (_isTaskCompleted) return;

        Debug.Log("HIT HIT HIT!");

        if (CoreManager.Instance != null)
        {
            CoreManager.Instance.StopAndLogCurrentTaskTime();
        }

        _isTaskCompleted = true;


        // Stop sound
        if (taskPanel && SoundManager.Instance && !taskPanel.panelToHide.activeSelf)
        {
            SoundManager.Instance.StopSound();
        }
        

        // Show the panel again
        if (CoreManager.Instance.currentTask == 1 && _trackingSwitcher != null)
            _trackingSwitcher.SwitchToHandsOnly();

        if (taskPanel && !taskPanel.panelToHide.activeSelf)
        {
            taskPanel.ShowPanel();

            if (proceedButton)
            {
                proceedButton.SetActive(true);
            }

            if (displayText)
            {
                displayText.gameObject.SetActive(true);

                var currentCondition = CoreManager.Instance.currentCondition;
                var currentTask = CoreManager.Instance.currentTask;

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
                    _ => "Unknown Cue"
                };

                displayText.text = $"TASK {currentTask}: {taskType} ({cueType})";
                interactionStatusText.text = "Status: Task Completed! You may proceed.";
                interactionStatusText.gameObject.SetActive(true);
            }

        }

        if (CoreManager.Instance.currentTask == 1)
            _trackingSwitcher.SwitchToHandsOnly();
    }

    public void revertAudioPosition()
    {
        if (audioRb != null)
        {
            audioRb.MovePosition(SoundManager.Instance._soundPosition);
            Debug.Log("Moved sound source to: " + _soundPosition);
        }

    }

    public void SetMeshMaterial(Material newMaterial)
    {
        foreach (GameObject envObject in environmentObjects)
        {
            Renderer renderer = envObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = newMaterial;
            }
        }
    }

}
