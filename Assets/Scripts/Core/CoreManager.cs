
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

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

    public bool _firstLog = true;

    private void Start()
    {
        if (_firstLog)
        {
            SceneManager.LoadScene("PracticeScene");
        }
        
    }

    public void SetFirstLog(bool value)
    {
        _firstLog = value;
    }
    
    public bool GetFirstLog()
    {
        return _firstLog;

    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _taskDurations = new Dictionary<string, float>();
        }
        else
        {
            Destroy(gameObject);
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

        string actualScene = (nextScene.Contains("PC_Task") || nextScene.Contains("GC_Task"))
            ? "Task" + currentTask
            : nextScene;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(actualScene);
        asyncLoad.allowSceneActivation = false;

        // Wait until the scene is almost loaded
        while (asyncLoad.progress < 0.9f)
            yield return null;

        // Instantly activate the scene when ready
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
            yield return null;

        _isTransitioning = false;
    }


    public string GetCurrentCondition() => currentCondition;
    public int GetCurrentTask() => currentTask;
    public Dictionary<string, float> GetTaskDurations() => _taskDurations;
}
