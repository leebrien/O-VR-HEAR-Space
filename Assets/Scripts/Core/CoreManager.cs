using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;


public class CoreManager : MonoBehaviour
{
    public static CoreManager Instance;

    public string currentCondition;
    public int currentTask;

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
        if (scene.name == "PracticeScene" || scene.name == "Task1")
            StartCoroutine(LinkDependenciesWhenReady());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator LinkDependenciesWhenReady()
    {
        // Wait until the rig references have been set by the initializer script.
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
            _leftSaber.transform.GetChild(0).gameObject.GetComponent<SaberBlade>().UpdateSceneReferences(_soundManager, null);
            _rightSaber.transform.GetChild(0).gameObject.GetComponent<SaberBlade>().UpdateSceneReferences(_soundManager, null);
        }
        else if (active.Contains("Task"))
        {
            Debug.Log("[Dependency] In Task scene, linking sound TaskInteraction to sabers." + SceneManager.GetActiveScene().name);
            _taskInteraction = FindFirstObjectByType<TaskInteraction>();
            _leftSaber.transform.GetChild(0).gameObject.GetComponent<SaberBlade>().UpdateSceneReferences(null, _taskInteraction);
            _rightSaber.transform.GetChild(0).gameObject.GetComponent<SaberBlade>().UpdateSceneReferences(null, _taskInteraction);
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
            _sceneOrder = new []
            {
                "GC_Task1", "Questionnaire", "PC_Task1", "Questionnaire", "Break",
                "GC_Task2", "Questionnaire", "PC_Task2", "Questionnaire", "Break",
                "GC_Task3", "Questionnaire", "PC_Task3", "Questionnaire", "End"
            };
        }
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
            int completedSceneIndex = _currentIndex - 1;
            string uniqueLogKey = $"{_runningTaskIdentifier}_Run_End_Index_{completedSceneIndex}";

            try
            {
                _taskDurations.Add(uniqueLogKey, duration);
                Debug.Log($"[CoreManager] TASK COMPLETED: {uniqueLogKey} = {duration:F2}s");
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError($"[CoreManager] Duplicate log key: {e.Message}");
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
        foreach (var entry in _taskDurations)
            Debug.Log($"Task: {entry.Key}, Duration: {entry.Value:F2}s");
        Debug.Log("===========================================");
    }

    // 🔹 SMOOTH TRANSITION (no fading)
    public void LoadNextScene()
    {
        if (_isTransitioning) return;
        if (_sceneOrder == null || _sceneOrder.Length == 0)
        {
            Debug.LogWarning("Scene order not built.");
            return;
        }
        if (_currentIndex >= _sceneOrder.Length)
        {
            Debug.Log("No more scenes to load.");
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

        if (nextScene == "End")
        {
            PrintAllTaskTimes();
        }

        Debug.Log($"[CoreManager] Preparing to load: {nextScene} (Condition={currentCondition}, Task={currentTask})");

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
                interactor.Disable();
            }
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

            while (!asyncLoad.isDone)
                yield return null;
        }
        
        foreach (var interactor in interactors)
        {
            interactor.Enable();
        }

        _isTransitioning = false;
    }


    public string GetCurrentCondition() => currentCondition;
    public int GetCurrentTask() => currentTask;
    public Dictionary<string, float> GetTaskDurations() => _taskDurations;
}
