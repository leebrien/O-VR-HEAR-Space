using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class QuestionData
{
    // Common fields
    public string id;
    public string type;
    public string text;
    public string lowLabel;
    public string highLabel;

    // Slider-specific fields
    public float minValue;
    public float maxValue;

    // Radio-specific field
    public int steps;
}

[Serializable]
public class QuestionnaireData
{
    public string questionnaireName;
    public List<QuestionData> questions;
}

public class QuestionnaireModel : MonoBehaviour
{
    public TextAsset jsonFile;

    private QuestionnaireData data;
    public int currentQuestionIndex { get; private set; }
    public Dictionary<string, float> sessionResponses = new Dictionary<string, float>();
    private string logFileName = "VR_Questionnaire_Data.csv";

    void Awake()
    {
        // Initialize with the file assigned in the inspector, if any
        if (jsonFile != null)
        {
            InitializeWithData(jsonFile);
        }
    }

    // Load and parse questionnaire file
    public void InitializeWithData(TextAsset newJsonFile)
    {
        jsonFile = newJsonFile;
        currentQuestionIndex = 0;
        sessionResponses.Clear(); // Clear previous questionnaire's answers

        if (jsonFile != null)
        {
            try
            {
                data = JsonUtility.FromJson<QuestionnaireData>(jsonFile.text);
                if (data == null || data.questions == null)
                {
                    data = new QuestionnaireData { questions = new List<QuestionData>() };
                    Debug.LogError("JSON Data is empty or questions list is missing/null.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize JSON in {jsonFile.name}: {e.Message}");
                data = new QuestionnaireData { questions = new List<QuestionData>() };
            }
        }
        else
        {
            data = new QuestionnaireData { questions = new List<QuestionData>() };
            Debug.LogError("JSON File is null. Cannot initialize model.");
        }

        Debug.Log($"Model Initialized with Questionnaire: {QuestionnaireName}");
    }

    public QuestionData GetCurrentQuestion()
    {
        if (data != null && data.questions != null && currentQuestionIndex >= 0 && currentQuestionIndex < data.questions.Count)
        {
            return data.questions[currentQuestionIndex];
        }
        return null;
    }

    public int GetTotalQuestions()
    {
        return (data != null && data.questions != null) ? data.questions.Count : 0;
    }

    public void AdvanceQuestion() => currentQuestionIndex++;
    public void RetreatQuestion() => currentQuestionIndex--;

    public void LogResponse(string qID, float score)
    {
        sessionResponses[qID] = score;
        Debug.Log($"Logged response for {qID}: {score}");
    }

    public void SubmitData()
    {
        List<string> finalLog = new List<string>();
        string filePath = Path.Combine(Application.persistentDataPath, logFileName);

        // If the file doesn't exist, add the header
        if (!File.Exists(filePath))
        {
            finalLog.Add("ParticipantID,Questionnaire,QuestionID,Score,Timestamp");
        }

        string participantID = "P001";
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        foreach (var response in sessionResponses)
        {
            // log
            string logEntry = $"{participantID},{QuestionnaireName},{response.Key},{response.Value},{timestamp}";
            finalLog.Add(logEntry);
        }

        // Append code
        File.AppendAllLines(filePath, finalLog);
        Debug.Log($"Data saved successfully to: {filePath}");
    }

    public string QuestionnaireName
    {
        get { return data != null ? data.questionnaireName : "No Questionnaire Loaded"; }
    }
    
}