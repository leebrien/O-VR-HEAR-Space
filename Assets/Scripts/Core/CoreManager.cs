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

    private string _sessionParticipantID;

    private int _currentIndex;
    private string[] _sceneOrder;

    private float _taskStartTime;
    private string _runningTaskIdentifier = string.Empty;
    //private Dictionary<string, float> _taskDurations;
    private Dictionary<string, string> _taskResults;
    
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
    public string GetSessionParticipantID() => _sessionParticipantID;
    


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


    public void SetFirstLog(bool value)
    {
        firstLog = value;
    }


    public bool GetFirstLog()
    {
        return firstLog;

    }

    private void Start()
    {
        SceneManager.LoadScene("PracticeScene");
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _taskResults = new Dictionary<string, string>();
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Calculate participant ID once on Awake
            _sessionParticipantID = CalculateNextParticipantID();
            Debug.Log($"[CoreManager] Session started for Participant ID: {_sessionParticipantID}");
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
        if (scene.name == "PracticeScene" || scene.name.Contains("Task1"))
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

    public void GenerateProtolSceneSequence(string setID)
    {
        BuildSceneOrder(setID);
        SceneLoader.LoadScene("LobbyScene");
    }

    private void BuildSceneOrder(string setID)
    {
        string[] rawTasks = GetTaskSequence(setID);
        List<string> protocolSequence = new List<string>();

        for (int i = 0; i < rawTasks.Length; i++)
        {
            protocolSequence.Add(rawTasks[i]);
            protocolSequence.Add("Questionnaire");
            if (i == 2) protocolSequence.Add("Break");
        }
        _sceneOrder = protocolSequence.ToArray();
        _currentIndex = 0;
        print( " Protocol Order"  + " " + _sceneOrder);
    }

    private string[] GetTaskSequence(string setID)
    {
        switch (setID)
        {
            case "1":
                return new[] {"GC_Task1", "PC_Task1", "GC_Task2", "PC_Task2", "GC_Task3", "PC_Task3"};
            case "2":
                return new[] {"PC_Task1", "GC_Task1", "PC_Task2", "GC_Task2", "PC_Task3", "GC_Task3"};
            case "3":
                return new[] { "GC_Task2", "PC_Task2", "GC_Task3", "PC_Task3", "GC_Task1", "PC_Task1" };
            case "4":
                return new[] { "PC_Task2", "GC_Task2", "PC_Task3", "GC_Task3", "PC_Task1", "GC_Task1" };
            case "5":
                return new[] { "GC_Task3", "PC_Task3", "GC_Task1", "PC_Task1", "GC_Task2", "PC_Task2" };
            case "6":
                return new[] { "PC_Task3", "GC_Task3", "PC_Task1", "GC_Task1", "PC_Task2", "GC_Task2" };
            default:
                return null;
        }
    }

    public void StartCurrentTaskTime()
    {
        string taskIdentifier = $"{currentCondition}_Task{currentTask}";
        _taskStartTime = Time.time;
        _runningTaskIdentifier = taskIdentifier;
        Debug.Log($"[CoreManager] Started timing for: {taskIdentifier} at {Time.time:F2}s");
    }

    public void StopAndLogCurrentTaskTime(bool isSuccess)
    {
        if (!string.IsNullOrEmpty(_runningTaskIdentifier) && _taskStartTime > 0f)
        {
            float duration = Time.time - _taskStartTime;
            int completedSceneIndex = _currentIndex;
            string uniqueLogKey = $"{_runningTaskIdentifier}_Run_End_Index_{completedSceneIndex}";

            string statusString = isSuccess ? "Success" : "Failed";
            string logValue = $"{duration:F2} | {statusString}";

            try
            {
                if (!_taskResults.ContainsKey(uniqueLogKey))
                {
                    _taskResults.Add(uniqueLogKey, logValue);
                    Debug.Log($"[CoreManager] TASK ENDED: {uniqueLogKey} = {logValue}");
                }
                else
                {
                    Debug.LogWarning($"[CoreManager] Attempted to log duplicate key: {uniqueLogKey}. Ignoring.");
                }
            }
            catch (ArgumentException e)
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
        Debug.Log($"Total Logged Entries: {_taskResults.Count}");
        foreach (var entry in _taskResults.OrderBy(kvp => kvp.Key))
            Debug.Log($"ID: {entry.Key} | Data: {entry.Value}");
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
    public Dictionary<string, string> GetTaskResults() => _taskResults;


    // Participant ID calculation
    private string CalculateNextParticipantID()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "VR_Questionnaire_Data.json");
        int nextParticipantNumber = 1;

        if (File.Exists(filePath))
        {
            try
            {
                string existingJson = File.ReadAllText(filePath);
                AllResultsData allResultsData = JsonUtility.FromJson<AllResultsData>(existingJson);

                if (allResultsData != null && allResultsData.participants != null && allResultsData.participants.Count > 0)
                {
                    int maxIdNumber = 0;
                    foreach (var p in allResultsData.participants)
                    {
                        if (p.participantID.StartsWith("P") && int.TryParse(p.participantID.Substring(1), out int num))
                        {
                            if (num > maxIdNumber) maxIdNumber = num;
                        }
                    }
                    nextParticipantNumber = maxIdNumber + 1;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuestionnaireModel] Error reading Participant ID: {e.Message}");
            }
        }

        return $"P{nextParticipantNumber:D3}";
    }
    

}