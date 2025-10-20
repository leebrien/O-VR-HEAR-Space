using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;


public class QuestionnaireModel : MonoBehaviour
{
    // Keep public if might assign for testing, otherwise make private
    // public TextAsset jsonFile;
    [SerializeField] private TextAsset jsonFile;

    private QuestionnaireData data;
    public int currentQuestionIndex { get; private set; }
    // Using float for numeric scores
    public Dictionary<string, float> sessionResponses = new Dictionary<string, float>();

    private string logFileName = "VR_Questionnaire_Data.json";


    public void InitializeWithData(TextAsset newJsonFile)
    {
        jsonFile = newJsonFile;
        currentQuestionIndex = 0;
        sessionResponses.Clear();

        if (newJsonFile != null) 
        {
            try
            {
                data = JsonUtility.FromJson<QuestionnaireData>(newJsonFile.text);
                if (data == null || data.questions == null)
                {
                    data = new QuestionnaireData { questions = new List<QuestionData>() };
                    Debug.LogError($"[QuestionnaireModel] JSON Data in '{newJsonFile.name}' is empty or questions list is missing/null.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuestionnaireModel] Failed to deserialize JSON in {newJsonFile.name}: {e.Message}");
                data = new QuestionnaireData { questions = new List<QuestionData>() };
            }
        }
        else
        {
            data = new QuestionnaireData { questions = new List<QuestionData>() };
            Debug.LogError("[QuestionnaireModel] JSON File provided to InitializeWithData is null.");
        }

        // Debug.Log($"[QuestionnaireModel] Initialized with Questionnaire: {QuestionnaireName}");
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
        //Debug.Log($"[QuestionnaireModel] Logged response for {qID}: {score}");
    }

    // Accepts participantID from Controller
    public void SubmitData(string participantID)
    {
        // 1. Create the new result object using the provided ID
        QuestionnaireResult newResult = new QuestionnaireResult
        {
            participantID = participantID,
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

                if (allResultsData == null || allResultsData.LogResponse == null)
                {
                    allResultsData = new AllResultsData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuestionnaireModel] Error reading existing results file: {e.Message}");
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
            string jsonToSave = JsonUtility.ToJson(allResultsData, true);
            File.WriteAllText(filePath, jsonToSave);
            Debug.Log($"[QuestionnaireModel] Data for {participantID} ({QuestionnaireName}) saved successfully to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestionnaireModel] Failed to save JSON data: {e.Message}");
        }
    }

    public string QuestionnaireName
    {
        get { return data != null ? data.questionnaireName : "No Questionnaire Loaded"; }
    }
}