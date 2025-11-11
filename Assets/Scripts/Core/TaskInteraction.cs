using System.Collections;
using UnityEngine;
using TMPro;

public class TaskInteraction : MonoBehaviour
{
    public Material successMaterial;
    public TaskPanel taskPanel;
    public GameObject[] environmentObjects;
    public GameObject proceedButton;
    public TextMeshProUGUI displayText;
    public TextMeshProUGUI interactionStatusText;
    public Rigidbody audioRb;
    public GameObject AudioGameObject;
    public AudioSource ambientAudioSource;

    private Vector3 _soundPosition;
    private TrackingSwitcher _trackingSwitcher;
    private SaberManager _saberManager;

    private string _sceneName;

    // Flag to ensure OnSuccess runs only once per task completion
    private bool _isTaskCompleted;

    private void Start()
    {
        _isTaskCompleted = false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void OnSuccess()
    {
        //Debug.Log("Entered On Success");
        // Guard clause
        if (_isTaskCompleted) return;
        _isTaskCompleted = true;
        
        CoreManager manager = CoreManager.Instance;
        SoundManager soundManager = SoundManager.Instance;

        //Debug.Log("HIT HIT HIT!");
        
        SetMeshMaterial(successMaterial);

        if (manager != null)
        {
            CoreManager.Instance.StopAndLogCurrentTaskTime();
        }
        
        // Stop sound
        if (taskPanel!= null && soundManager != null && !taskPanel.panelToHide.activeSelf)
        {
            soundManager.StopSound();
        }
        
        if (manager.currentTask == 1)
        {
            // Find the objects right before we use them
            _saberManager = FindFirstObjectByType<SaberManager>();
            if (_saberManager != null) 
            {
                _saberManager.DisableSabers();
            }
            
        }
        
        if (manager.currentTask == 3 && ambientAudioSource != null)
                ambientAudioSource.Stop();
        
        _trackingSwitcher = FindFirstObjectByType<TrackingSwitcher>();
        if (_trackingSwitcher != null)
        {
            _trackingSwitcher.SwitchToBoth();
        }
            
        StartCoroutine(ShowPanelAfterDelay());
    }
    
    private IEnumerator ShowPanelAfterDelay()
    {
        // 1. Wait for 2 seconds
        yield return new WaitForSeconds(2.0f);

        // 2. Hide the audio object
        if (AudioGameObject != null)
        {
            AudioGameObject.SetActive(false);
        }

        // 3. Now, show the panel and set up all its text
        if (taskPanel != null && !taskPanel.panelToHide.activeSelf)
        {
            taskPanel.ShowPanel();

            if (proceedButton != null)
            {
                proceedButton.SetActive(true);
            }

            if (displayText != null)
            {
                displayText.gameObject.SetActive(true);

                // We get the manager instance here, inside the coroutine
                var manager = CoreManager.Instance; 
                var currentCondition = manager.currentCondition;
                var currentTask = manager.currentTask;

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
    }
    
    
    

    // ReSharper disable Unity.PerformanceAnalysis
    public void SetMeshMaterial(Material newMaterial)
    {
        // First, check if the array itself is null
        if (environmentObjects == null) return;

        foreach (GameObject envObject in environmentObjects)
        {
            // Add this check!
            if (envObject != null)
            {
                Renderer renderer = envObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = newMaterial;
                }
            }
        }
    }
}

