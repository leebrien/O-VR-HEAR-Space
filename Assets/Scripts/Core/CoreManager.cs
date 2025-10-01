using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CoreManager : MonoBehaviour
{
    public static CoreManager Instance;

    // Current experiment condition (PC = personalized, GC = generic)
    public string currentCondition;

    // Current task number (1,2,3)
    public int currentTask;

    // Keeps track of progress in scene order
    public int _currentIndex = 0;

    // The full sequence of scenes to run
    private string[] _sceneOrder;

    // Time logging fields
    private float _taskStartTime = 0f;
    //Task identifier
    private string _runningTaskIdentifier = string.Empty;
    //Time logger
    private Dictionary<string, float> _taskDurations;

    private void Awake()
    {
        //Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // survive scene transitions
            _taskDurations = new Dictionary<string, float>(); // Initialize the dictionary
        }
        else
        {
            Destroy(gameObject); // kill duplicates
        }
    }

    private void SetCondition(string condition)
    {
        currentCondition = condition;
    }

    private void SetTask(int task)
    {
        currentTask = task;
    }

    // Starting condition setter
    public void SetStartingCondition(string condition) // kabit sa panel sa option ng cue what to go first
    {
        SetCondition(condition); // "PC" "GC"
        BuildSceneOrder();
    }

    // Build sequence of scenes depending on starting condition
    private void BuildSceneOrder()
    {
        if (currentCondition == "PC")
        {
            _sceneOrder = new string[]
            {
                "PC_Task1", "Questionnaire", "GC_Task1", "Questionnaire", "Break",
                "PC_Task2", "Questionnaire", "GC_Task2", "Questionnaire", "Break",
                "PC_Task3", "Questionnaire", "GC_Task3", "Questionnaire", "End"
            };
        }
        else if (currentCondition == "GC")
        {
            _sceneOrder = new string[]
            {
                "GC_Task1", "Questionnaire", "PC_Task1", "Questionnaire", "Break",
                "GC_Task2", "Questionnaire", "PC_Task2", "Questionnaire", "Break",
                "GC_Task3", "Questionnaire", "PC_Task3", "Questionnaire", "End"
            };
        }
    }

    private bool IsTaskScene(string sceneName)
    {
        return sceneName.Contains("PC_Task") || sceneName.Contains("GC_Task");
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

            // Add the duration to the dictionary and log completion
            try
            {
                _taskDurations.Add(uniqueLogKey, duration);
                Debug.Log($"[CoreManager] TASK COMPLETED AND LOGGED: {uniqueLogKey} = {duration:F2}s");
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError($"[CoreManager] Failed to log task time: Key already exists. {e.Message}");
            }

            // Reset timer state
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
        {
            Debug.Log($"Task: {entry.Key}, Duration: {entry.Value:F2}s");
        }
        Debug.Log("===========================================");
    }

    // Loads the next scene in the sequence
    public void LoadNextScene()
    {
        if (_sceneOrder == null || _sceneOrder.Length == 0)
        {
            Debug.LogWarning("Scene order not built.");
            return;
        }

        if (_currentIndex < _sceneOrder.Length)
        {
            string nextScene = _sceneOrder[_currentIndex];

            // Extract condition + task from scene name
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

            // print the final log
            if (nextScene == "End")
            {
                PrintAllTaskTimes();
            }

            Debug.Log($"[CoreManager] Loading scene: {nextScene}, Condition={currentCondition}, Task={currentTask}");


            // For task scenes, load the "TaskX" scene
            if (nextScene.Contains("PC_Task") || nextScene.Contains("GC_Task"))
            {
                SceneManager.LoadScene("Task" + currentTask);
            }
            else
            {
                SceneManager.LoadScene(nextScene);
            }

            _currentIndex++;
        }
        else
        {
            Debug.Log("No more scenes to load.");
        }
    }

    public string GetCurrentCondition()
    {
        return currentCondition;
    }

    public int GetCurrentTask()
    {
        return currentTask;
    }

    public Dictionary<string, float> GetTaskDurations()
    {
        return _taskDurations;
    }
}
