using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class QuestionnaireManager : MonoBehaviour
{
    [Header("Questionnaire Files")]
    [SerializeField] private List<TextAsset> questionnaireSequence;

    [Header("References")]
    [SerializeField] private QuestionnaireController questionnaireController;

    private int currentSequenceIndex = 0; // Tracks progress through the questionnaireSequence list
    private string sessionParticipantID;

    // Flags to ensure Cue Preference runs only once per task pair trigger point
    private bool cuePreferenceDoneAfterIndex3 = false; // Triggered when CoreManager index becomes 4
    private bool cuePreferenceDoneAfterIndex8 = false; // Triggered when CoreManager index becomes 9
    private bool cuePreferenceDoneAfterIndex13 = false; // Triggered when CoreManager index becomes 14

    void Start()
    {
        // Get Participant ID from CoreManager
        if (CoreManager.Instance != null)
        {
            sessionParticipantID = CoreManager.Instance.GetSessionParticipantID();
            Debug.Log($"[QuestionnaireManager] Using Participant ID: {sessionParticipantID}");
        }
        else
        {
            Debug.LogError("[QuestionnaireManager] CoreManager instance not found! Cannot get Participant ID.");
            sessionParticipantID = "P_ERROR"; // Fallback ID
        }

        QuestionnaireController.OnQuestionnaireCompleted += HandleQuestionnaireCompleted;

        if (questionnaireController != null)
        {
            StartNextQuestionnaire();
        }
        else
        {
            Debug.LogError("[QuestionnaireManager] QuestionnaireController not assigned in Inspector!");
        }
    }

    private void OnDestroy()
    {

            QuestionnaireController.OnQuestionnaireCompleted -= HandleQuestionnaireCompleted;
       
    }

    private void StartNextQuestionnaire()
    {
        if (currentSequenceIndex >= questionnaireSequence.Count)
        {
            Debug.Log($"[QuestionnaireManager] Reached end of questionnaire sequence for {sessionParticipantID}. Loading next scene via CoreManager.");
            ProceedToNextCoreScene();
            return;
        }

        TextAsset nextFile = questionnaireSequence[currentSequenceIndex];
        if (nextFile.name == "cp")
        {
            int coreManagerCurrentIndex = -1;
            if (CoreManager.Instance != null)
            {
                coreManagerCurrentIndex = CoreManager.Instance.GetCurrentSceneIndex();
            }
            else
            {
                Debug.LogError("[QuestionnaireManager] CoreManager instance not found! Cannot check index for CP.");
                //currentSequenceIndex++;
                StartNextQuestionnaire();
                return;
            }

            // Determine if it's the RIGHT TIME to show cuePreference
            bool runCpNow = false;
            if (coreManagerCurrentIndex == 4 && !cuePreferenceDoneAfterIndex3)
            {
                runCpNow = true;
                cuePreferenceDoneAfterIndex3 = true;
            }
            else if (coreManagerCurrentIndex == 9 && !cuePreferenceDoneAfterIndex8)
            {
                runCpNow = true;
                cuePreferenceDoneAfterIndex8 = true;
            }
            else if (coreManagerCurrentIndex == 14 && !cuePreferenceDoneAfterIndex13)
            {
                runCpNow = true;
                cuePreferenceDoneAfterIndex13 = true;
            }

            if (runCpNow)
            {
                Debug.Log($"[QuestionnaireManager] Triggering Cue Preference (CP.json) found at sequence index {currentSequenceIndex}. CoreManager index is {coreManagerCurrentIndex}.");
                questionnaireController.StartQuestionnaire(nextFile, sessionParticipantID);
            }
            else
            {
                Debug.Log($"[QuestionnaireManager] Skipping Cue Preference (CP.json) found at sequence index {currentSequenceIndex}. CoreManager index is {coreManagerCurrentIndex}. Required index 4/9/14 or already done.");
                currentSequenceIndex++; 
                StartNextQuestionnaire(); 
                return;
            }
        }
        else
        {
            questionnaireController.StartQuestionnaire(nextFile, sessionParticipantID);
        }
    }

    private void HandleQuestionnaireCompleted()
    {
        currentSequenceIndex++;
        StartNextQuestionnaire();
    }

    // Helper function to call CoreManager's LoadNextScene safely
    private void ProceedToNextCoreScene()
    {
        if (CoreManager.Instance != null)
        {
            CoreManager.Instance.LoadNextScene();
        }
        else
        {
            Debug.LogError("[QuestionnaireManager] CoreManager not found, cannot proceed to next scene automatically.");
        }
    }
}