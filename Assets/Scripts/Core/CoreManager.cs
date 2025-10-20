using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;
using System.IO;
using System;

public class CoreManager : MonoBehaviour
{
    public static CoreManager Instance;

    public string currentCondition;
    public int currentTask;

    private string sessionParticipantID;

    private int _currentIndex;
    private string[] _sceneOrder;

    private float _taskStartTime;
    private string _runningTaskIdentifier = string.Empty;
    private Dictionary<string, float> _taskDurations;

    private bool _isTransitioning;
    private bool _rigReferencesSet;

    public bool firstLog = true;

    private GameObject _ovrRig;
    private GameObject _ovrHands;
    private GameObject _ovrControllers;
    private GameObject _leftXRRayInteractor;
    private GameObject _rightXRRayInteractor;
    private GameObject _leftSaber;
    private GameObject _rightSaber;
    private Transform _centerEyeAnchor;

    public GameObject GetRig() => _ovrRig;
    public GameObject GetHands() => _ovrHands;
    public GameObject GetControllers() => _ovrControllers;

    public GameObject GetLeftXRRayInteractor() => _leftXRRayInteractor;
    public GameObject GetRightXRRayInteractor() => _rightXRRayInteractor;
    public GameObject GetLeftSaber() => _leftSaber;
    public GameObject GetRightSaber() => _rightSaber;
    public Transform GetCenterEyeAnchor() => _centerEyeAnchor;
    public string GetSessionParticipantID() => sessionParticipantID;


    private PracticeSoundManager _soundManager;
    private TaskInteraction _taskInteraction;


    public void SetRigReferences(GameObject rig, GameObject hands, GameObject controllers,
        GameObject leftXRRayInteractor, GameObject rightXRRayInteractor,
        GameObject leftSaber, GameObject rightSaber,
        Transform centerEyeAnchor)
    {
        Debug.Log("[CoreManager] Setting rig references." +
                    "OVR Rig: " + rig +
                    ", Hands: " + hands +
                    ", Controllers: " + controllers +
                    ", Left XR Ray Interactor: " + leftXRRayInteractor +
                    ", Right XR Ray Interactor: " + rightXRRayInteractor +
                    ", Left Saber: " + leftSaber +
                    ", Right Saber: " + rightSaber +
                    ", Center Eye Anchor: " + centerEyeAnchor);

        _ovrRig = rig;
        _ovrHands = hands;
        _ovrControllers = controllers;
        _leftXRRayInteractor = leftXRRayInteractor;
        _rightXRRayInteractor = rightXRRayInteractor;
        _leftSaber = leftSaber;
        _rightSaber = rightSaber;
        _centerEyeAnchor = centerEyeAnchor;
        _rigReferencesSet = true;
    }

    private void Start()
    {
        SceneManager.LoadScene(firstLog ? "PracticeScene" : "LobbyScene");
    }


    public void SetFirstLog(bool value)
    {
        firstLog = value;
    }


    public bool GetFirstLog()
    {
        return firstLog;

    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _taskDurations = new Dictionary<string, float>();
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Calculate participant ID once on Awake
            sessionParticipantID = CalculateNextParticipantID();
            Debug.Log($"[CoreManager] Session started for Participant ID: {sessionParticipantID}");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if scene name contains "Task" instead of just "Task1"
        if (scene.name == "PracticeScene" || scene.name.Contains("Task"))
            StartCoroutine(LinkDependenciesWhenReady());
    }

    private IEnumerator LinkDependenciesWhenReady()
    {
        yield return new WaitUntil(() => _rigReferencesSet);
        LinkDependenciesImmediate();
    }


    private void LinkDependenciesImmediate()
    {
        var active = SceneManager.GetActiveScene().name;

        if (active.Contains("Practice"))
        {
            Debug.Log("[Dependency] In practice scene, linking sound manager to sabers." + SceneManager.GetActiveScene().name);
            _soundManager = FindFirstObjectByType<PracticeSoundManager>();
            if (_leftSaber != null && _soundManager != null)
                _leftSaber.transform.GetChild(0).gameObject.GetComponent<SaberBlade>()?.UpdateSceneReferences(_soundManager, null);
            if (_rightSaber != null && _soundManager != null)
                _rightSaber.transform.GetChild(0).gameObject.GetComponent<SaberBlade>()?.UpdateSceneReferences(_soundManager, null);
        }
        else if (active.Contains("Task"))
        {
            Debug.Log("[Dependency] In Task scene, linking TaskInteraction to sabers." + SceneManager.GetActiveScene().name);
            _taskInteraction = FindFirstObjectByType<TaskInteraction>();

            if (_leftSaber != null && _taskInteraction != null)
                _leftSaber.transform.GetChild(0).gameObject.GetComponent<SaberBlade>()?.UpdateSceneReferences(null, _taskInteraction);
            if (_rightSaber != null && _taskInteraction != null)
                _rightSaber.transform.GetChild(0).gameObject.GetComponent<SaberBlade>()?.UpdateSceneReferences(null, _taskInteraction);
        }

    }

    private void SetCondition(string condition) => currentCondition = condition;
    private void SetTask(int task) => currentTask = task;

    public void SetStartingCondition(string condition)
    {
        SetCondition(condition);
        BuildSceneOrder();
    }

    private void BuildSceneOrder()
    {
        if (currentCondition == "PC")
        {
            _sceneOrder = new[]
            {
                "PC_Task1", "Questionnaire", "GC_Task1", "Questionnaire", "Break",
                "PC_Task2", "Questionnaire", "GC_Task2", "Questionnaire", "Break",
                "PC_Task3", "Questionnaire", "GC_Task3", "Questionnaire", "End"
            };
        }
        else if (currentCondition == "GC")
        {
            _sceneOrder = new[]
            {
                "GC_Task1", "Questionnaire", "PC_Task1", "Questionnaire", "Break",
                "GC_Task2", "Questionnaire", "PC_Task2", "Questionnaire", "Break",
                "GC_Task3", "Questionnaire", "PC_Task3", "Questionnaire", "End"
            };
        }
        // Reset scene index when building order
        _currentIndex = 0;
    }

    public void StartCurrentTaskTime()
    {
        string taskIdentifier = $"{currentCondition}_Task{currentTask}";
        _taskStartTime = Time.time;
        _runningTaskIdentifier = taskIdentifier;
        Debug.Log($"[CoreManager] Started timing for: {taskIdentifier} at {Time.time:F2}s");
    }

    public void StopAndLogCurrentTaskTime()
    {
        if (!string.IsNullOrEmpty(_runningTaskIdentifier) && _taskStartTime > 0f)
        {
            float duration = Time.time - _taskStartTime;
            int completedSceneIndex = _currentIndex;
            string uniqueLogKey = $"{_runningTaskIdentifier}_Run_End_Index_{completedSceneIndex}";

            try
            {
                if (!_taskDurations.ContainsKey(uniqueLogKey))
                {
                    _taskDurations.Add(uniqueLogKey, duration);
                    Debug.Log($"[CoreManager] TASK COMPLETED: {uniqueLogKey} = {duration:F2}s");
                }
                else
                {
                    Debug.LogWarning($"[CoreManager] Attempted to log duplicate key: {uniqueLogKey}. Ignoring.");
                }

            }
            catch (System.ArgumentException e)
            {
                Debug.LogError($"[CoreManager] Error adding log key: {e.Message}");
            }

            _taskStartTime = 0f;
            _runningTaskIdentifier = string.Empty;
        }
    }

    public void PrintAllTaskTimes()
    {
        Debug.Log("===========================================");
        Debug.Log("[CoreManager] --- FINAL TASK TIME LOG ---");
        Debug.Log($"Total Logged Entries: {_taskDurations.Count}");
        foreach (var entry in _taskDurations.OrderBy(kvp => kvp.Key))
            Debug.Log($"Task: {entry.Key}, Duration: {entry.Value:F2}s");
        Debug.Log("===========================================");
    }

    // 🔹 SMOOTH TRANSITION (no fading)
    public void LoadNextScene()
    {
        if (_isTransitioning) return;
        if (_sceneOrder == null || _sceneOrder.Length == 0)
        {
            Debug.LogWarning("[CoreManager] Scene order not built. Cannot load next scene.");
            return;
        }
        if (_currentIndex >= _sceneOrder.Length)
        {
            Debug.Log("[CoreManager] End of scene order reached.");
            PrintAllTaskTimes();
            return;
        }

        string nextScene = _sceneOrder[_currentIndex];
        _currentIndex++;

        if (nextScene.Contains("PC_Task"))
        {
            SetCondition("PC");
            if (int.TryParse(nextScene.Replace("PC_Task", ""), out int taskNum))
                SetTask(taskNum);
        }
        else if (nextScene.Contains("GC_Task"))
        {
            SetCondition("GC");
            if (int.TryParse(nextScene.Replace("GC_Task", ""), out int taskNum))
                SetTask(taskNum);
        }


        Debug.Log($"[CoreManager] Preparing to load scene index {_currentIndex - 1}: {nextScene} (Condition={currentCondition}, Task={currentTask})");

        StartCoroutine(PreloadAndActivateScene(nextScene));
    }



    private IEnumerator PreloadAndActivateScene(string nextScene)
    {
        _isTransitioning = true;

        List<IInteractor> interactors = new List<IInteractor>();
        if (_ovrRig != null)
        {
            interactors = _ovrRig.GetComponentsInChildren<IInteractor>(true).ToList();
            foreach (var interactor in interactors)
            {
                if (interactor != null) interactor.Disable();
            }
        }
        else
        {
            Debug.LogWarning("[CoreManager] OVR Rig reference not set, cannot disable interactors.");
        }

        string actualScene = (nextScene.Contains("PC_Task") || nextScene.Contains("GC_Task"))
            ? "Task" + currentTask
            : nextScene;


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(actualScene);
        if (asyncLoad != null)
        {
            asyncLoad.allowSceneActivation = false;

            // Wait until the scene is almost loaded
            while (asyncLoad.progress < 0.9f)
                yield return null;

            // Instantly activate the scene when ready
            asyncLoad.allowSceneActivation = true;

            // Wait for activation to complete
            while (!asyncLoad.isDone)
                yield return null;
        }
        else
        {
            Debug.LogError($"[CoreManager] Failed to start loading scene: {actualScene}");
        }

        foreach (var interactor in interactors)
        {
            if (interactor != null)
            {
                interactor.Enable();
            }
        }

        _isTransitioning = false;
    }


    public string GetCurrentCondition() => currentCondition;
    public int GetCurrentTask() => currentTask;

    public int GetCurrentSceneIndex() => _currentIndex;
    public Dictionary<string, float> GetTaskDurations() => _taskDurations;


    // Participant ID calculation
    private string CalculateNextParticipantID()
    {
        string logFileName = "VR_Questionnaire_Data.json";
        string filePath = Path.Combine(Application.persistentDataPath, logFileName);
        int nextParticipantNumber = 1;

        if (File.Exists(filePath))
        {
            try
            {
                string existingJson = File.ReadAllText(filePath);
                AllResultsData allResultsData = JsonUtility.FromJson<AllResultsData>(existingJson);

                if (allResultsData != null && allResultsData.LogResponse != null && allResultsData.LogResponse.Count > 0)
                {
                    int maxIdNumber = 0;
                    foreach (var result in allResultsData.LogResponse)
                    {
                        if (result.participantID != null && result.participantID.StartsWith("P"))
                        {
                            string numberPart = result.participantID.Substring(1);
                            if (int.TryParse(numberPart, out int idNumber))
                            {
                                if (idNumber > maxIdNumber)
                                {
                                    maxIdNumber = idNumber;
                                }
                            }
                        }
                    }
                    nextParticipantNumber = maxIdNumber + 1;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[CoreManager] Error reading results file to determine Participant ID: {e.Message}");
            }
        }
        return $"P{nextParticipantNumber:D3}";
    }
}