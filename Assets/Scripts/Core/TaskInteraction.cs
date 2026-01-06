using System.Collections;
using UnityEngine;
using TMPro;

public class TaskInteraction : MonoBehaviour
{
    public Material successMaterial;
    public Material failureMaterial;
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
    private bool _isTaskCompleted;

    private float _timeLimit;
    private float _localStartTime;
    private bool _isTimerRunning = false;

    private void Start()
    {
        _isTaskCompleted = false;
        _isTimerRunning = false;
        int currentTask = CoreManager.Instance != null ? CoreManager.Instance.currentTask : 1;
        
        if (currentTask == 3) _timeLimit = 600f; 
        else _timeLimit = 300f;
        Debug.Log($"[TaskInteraction] Timer started. Limit: {_timeLimit} seconds.");
    }
    
    void Update()
    {
        if (!_isTimerRunning || _isTaskCompleted) return;

        float elapsed = Time.time - _localStartTime;
        if (elapsed >= _timeLimit)
        {
            OnFailure();
        }
    }
    
    public void OnFailure()
    {
        if (_isTaskCompleted) return;
        _isTaskCompleted = true;
        _isTimerRunning = false;

        Debug.Log("[TaskInteraction] Time limit reached! Task Failed.");
        
        SetMeshMaterial(failureMaterial);
        
        CoreManager manager = CoreManager.Instance;
        if (manager != null)
        {
            manager.StopAndLogCurrentTaskTime(false); // false = Failed
        }
        
        CleanUpAndShowPanel(false); 
    }

    public void ActivateTaskTimer()
    {
        _localStartTime = Time.time;
        _isTimerRunning = true;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void OnSuccess()
    {
        if (_isTaskCompleted) return;
        _isTaskCompleted = true;
        _isTimerRunning = false;
        
        SetMeshMaterial(successMaterial);
        CoreManager manager = CoreManager.Instance;
        if (manager != null)
        {
            manager.StopAndLogCurrentTaskTime(true);
        }
        
        CleanUpAndShowPanel(true);
    }

    private void CleanUpAndShowPanel(bool isSuccess)
    {
        CoreManager manager = CoreManager.Instance;
        SoundManager soundManager = SoundManager.Instance;
        
        // Stop sound
        if (taskPanel!= null && soundManager != null && !taskPanel.panelToHide.activeSelf)
        {
            soundManager.StopSound();
        }
        
        if (manager.currentTask == 1)
        {
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
            
        StartCoroutine(ShowPanelAfterDelay(isSuccess));
    }
    
    
    private IEnumerator ShowPanelAfterDelay(bool isSuccess)
    {
        yield return new WaitForSeconds(2.0f);

        if (AudioGameObject != null)
        {
            AudioGameObject.SetActive(false);
        }

        if (taskPanel != null && !taskPanel.panelToHide.activeSelf)
        {
            taskPanel.ShowPanel();

            if (proceedButton != null) proceedButton.SetActive(true);
            if (displayText != null)
            {
                displayText.gameObject.SetActive(true);

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

                // Update text based on success or failure
                string statusMsg = isSuccess ? "Task Completed! You may proceed." : "Time Limit Reached. You may proceed.";
                
                displayText.text = $"TASK {currentTask}: {taskType} ({cueType})";
                interactionStatusText.text = $"Status: {statusMsg}";
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

