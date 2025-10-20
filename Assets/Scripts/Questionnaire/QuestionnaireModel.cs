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

[Serializable]
public class ResponseItem
{
    public string id;
    public float score; 

    public ResponseItem(string id, float score)
    {
        this.id = id;
        this.score = score;
    }
}

[Serializable]
public class QuestionnaireResult
{
    public string participantID;
    public string questionnaireName;
    public string timestamp;
    public List<ResponseItem> responses;
}

[Serializable]
public class AllResultsData
{
    public List<QuestionnaireResult> LogResponse;

    public AllResultsData()
    {
        LogResponse = new List<QuestionnaireResult>();
    }
}

// Main Model

public class QuestionnaireModel : MonoBehaviour
{
    public TextAsset jsonFile;

    private QuestionnaireData data;
    public int currentQuestionIndex { get; private set; }
    public Dictionary<string, float> sessionResponses = new Dictionary<string, float>();

    private string logFileName = "VR_Questionnaire_Data.json";


    public void InitializeWithData(TextAsset newJsonFile)
    {
        jsonFile = newJsonFile;
        currentQuestionIndex = 0;
        sessionResponses.Clear();

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
        // Debug.Log($"Model Initialized with Questionnaire: {QuestionnaireName}");
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
        //Debug.Log($"Logged response for {qID}: {score}");
    }

    public void SubmitData(string participantID) // <-- Parameter added
    {

        // 1. Create the new result object using the provided ID
        QuestionnaireResult newResult = new QuestionnaireResult
        {
            participantID = participantID, // Use the ID passed from the controller
            questionnaireName = this.QuestionnaireName,
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            responses = new List<ResponseItem>()
        };

        // 2. Convert sessionResponses dictionary
        foreach (var response in sessionResponses)
        {
            newResult.responses.Add(new ResponseItem(response.Key, response.Value));
        }

        // 3. Load existing results file (if it exists)
        string filePath = Path.Combine(Application.persistentDataPath, logFileName);
        AllResultsData allResultsData;

        if (File.Exists(filePath))
        {
            try
            {
                string existingJson = File.ReadAllText(filePath);
                allResultsData = JsonUtility.FromJson<AllResultsData>(existingJson);
                // More robust check for null or empty list
                if (allResultsData == null || allResultsData.LogResponse == null)
                {
                    allResultsData = new AllResultsData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading existing results file: {e.Message}");
                allResultsData = new AllResultsData();
            }
        }
        else
        {
            allResultsData = new AllResultsData();
        }

        // 4. Add the new result to the list
        allResultsData.LogResponse.Add(newResult);

        // 5. Serialize and overwrite the file
        try
        {
            string jsonToSave = JsonUtility.ToJson(allResultsData, true); // Pretty print enabled
            File.WriteAllText(filePath, jsonToSave);
            Debug.Log($"Data for {participantID} saved successfully to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save JSON data: {e.Message}");
        }
    }

    public string QuestionnaireName
    {
        get { return data != null ? data.questionnaireName : "No Questionnaire Loaded"; }
    }
}