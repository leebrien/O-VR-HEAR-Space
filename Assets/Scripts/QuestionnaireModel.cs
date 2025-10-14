using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

// JSON data structure for deserialization
[Serializable]
public class QuestionData
{
    // The fields in a single question item in the JSON array
    public string id;
    public string text;
    public float minValue;
    public float maxValue;
    public string lowLabel;
    public string highLabel;
}

[Serializable]
public class QuestionnaireData
{
    // Survey and question list
    public string questionnaireName;
    public List<QuestionData> questions;
}

public class QuestionnaireModel : MonoBehaviour
{
    // Assign JSON in insepctor
    public TextAsset jsonFile;

    private QuestionnaireData data;
    public int currentQuestionIndex { get; private set; } = 0;

    // It maps a question ID to a score to save responses
    public Dictionary<string, float> sessionResponses = new Dictionary<string, float>();

    private string logFileName = "VR_Questionnaire_Data.csv";

    void Awake()
    {
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
                Debug.LogError($"Failed to deserialize JSON: {e.Message}");
                data = new QuestionnaireData { questions = new List<QuestionData>() };
            }
        }
        else
        {
            data = new QuestionnaireData { questions = new List<QuestionData>() };
            Debug.LogError("JSON File not assigned. Initializing with no questions.");
        }
    }

    // Core Model Methods
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

    // Builds final log from dictionary
    public void SubmitData()
    {
        List<string> finalLog = new List<string>();
        finalLog.Add("ParticipantID,QuestionID,Score,Timestamp"); // Header

        string participantID = "P001"; // This should be made dynamic later
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Loop through the responses to build the final CSV data
        foreach (var response in sessionResponses)
        {
            string logEntry = $"{participantID},{response.Key},{response.Value},{timestamp}";
            finalLog.Add(logEntry);
        }

        string filePath = Path.Combine(Application.persistentDataPath, logFileName);
        File.WriteAllLines(filePath, finalLog);
        Debug.Log($"Data saved successfully to: {filePath}");
    }

    public string QuestionnaireName
    {
        get { return data != null ? data.questionnaireName : "Default Questionnaire"; }
    }

    public List<QuestionData> GetQuestions()
    {
        return data.questions;
    }
}