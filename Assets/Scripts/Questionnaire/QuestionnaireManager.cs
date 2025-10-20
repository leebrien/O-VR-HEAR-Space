using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class QuestionnaireManager : MonoBehaviour
{
    [SerializeField] private List<TextAsset> questionnaireSequence;
    [SerializeField] private QuestionnaireController questionnaireController;

    private int currentQuestionnaireIndex = 0;
    private string sessionParticipantID;

    void Start()
    {
        sessionParticipantID = GetNextParticipantID();
        Debug.Log($"Starting session for Participant ID: {sessionParticipantID}");

        QuestionnaireController.OnQuestionnaireCompleted += HandleQuestionnaireCompleted;
        StartNextQuestionnaire();
    }

    private void OnDestroy()
    {
        QuestionnaireController.OnQuestionnaireCompleted -= HandleQuestionnaireCompleted;
    }

    private void StartNextQuestionnaire()
    {
        if (currentQuestionnaireIndex < questionnaireSequence.Count)
        {
            //Debug.Log($" Starting Questionnaire {currentQuestionnaireIndex + 1}");
            TextAsset nextFile = questionnaireSequence[currentQuestionnaireIndex];
            questionnaireController.StartQuestionnaire(nextFile, sessionParticipantID);
        }
        else
        {
            //Debug.Log("There are no more questionnaires.");
        }
    }

    private void HandleQuestionnaireCompleted()
    {
        //Debug.Log("Questionnaire Completed");
        currentQuestionnaireIndex++;
        StartNextQuestionnaire();
    }

    private string GetNextParticipantID()
    {
        string logFileName = "VR_Questionnaire_Data.json"; // Make sure this matches the model
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
                Debug.LogError($"Error reading results file to determine Participant ID: {e.Message}");
                // Proceed with P001 in case of error
            }
        }
        // Format as P001, P002, etc.
        return $"P{nextParticipantNumber:D3}";
    }

}