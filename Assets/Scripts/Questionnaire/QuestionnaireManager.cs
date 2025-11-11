using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class QuestionnaireManager : MonoBehaviour
{
    [Header("Questionnaire Files")]
    [SerializeField] private List<TextAsset> questionnaireSequence;

    [Header("References")]
    [SerializeField] private QuestionnaireController questionnaireController;
    [Header("References")]
    [SerializeField]private SSQPanelController ssqPanelControllerEnding;

    private int _currentSequenceIndex; // Tracks progress through the questionnaireSequence list
    private string _sessionParticipantID;
    private string _currentCondition;
    private int _currentTask;

    // Flags to ensure Cue Preference runs only once per task pair trigger point
    private bool _cuePreferenceDoneAfterIndex3; // Triggered when CoreManager index becomes 4
    private bool _cuePreferenceDoneAfterIndex8; // Triggered when CoreManager index becomes 9
    private bool _cuePreferenceDoneAfterIndex13; // Triggered when CoreManager index becomes 14
    

    void Start()
    {
        // Get Participant ID from CoreManager
        if (CoreManager.Instance != null)
        {
            _sessionParticipantID = CoreManager.Instance.GetSessionParticipantID();
            if (CoreManager.Instance.GetSSQLog() == 0 && SceneManager.GetActiveScene().name == "SSQ-Scene")
            {
                Debug.Log("[QUESTIONMANAGER DebugLog1 :]"+ CoreManager.Instance.GetSSQLog());
                _currentCondition = "First SSQ";
                _currentTask = 0;
            }
            else if (CoreManager.Instance.GetSSQLog() == 1 && SceneManager.GetActiveScene().name == "SSQ-Scene")
            {
                Debug.Log("[QUESTIONMANAGER DebugLog2 :]"+ CoreManager.Instance.GetSSQLog());
                _currentCondition = "Second SSQ";
                _currentTask = 0;
            }
            else
            {
                _currentCondition = CoreManager.Instance.GetCurrentCondition();
                _currentTask = CoreManager.Instance.GetCurrentTask();
            }

            Debug.Log($"[QuestionnaireManager] Using Participant ID: {_sessionParticipantID}");
        }
        else
        {
            Debug.LogError("[QuestionnaireManager] CoreManager instance not found! Cannot get Participant ID.");
            _sessionParticipantID = "P_ERROR"; // Fallback ID
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
        if (_currentSequenceIndex >= questionnaireSequence.Count)
        {
            //Debug.Log($"[QuestionnaireManager] Reached end of questionnaire sequence for {_sessionParticipantID}. Loading next scene via CoreManager.");
            ProceedToNextCoreScene();
            return;
        }

        TextAsset nextFile = questionnaireSequence[_currentSequenceIndex];
        if (nextFile.name == "cp")
        {
            int coreManagerCurrentIndex = -1;
            if (CoreManager.Instance != null)
            {
                coreManagerCurrentIndex = CoreManager.Instance.GetCurrentSceneIndex();
            }
            else
            {
                //Debug.LogError("[QuestionnaireManager] CoreManager instance not found! Cannot check index for CP.");
                //currentSequenceIndex++;
                StartNextQuestionnaire();
                return;
            }

            // Determine if it's the RIGHT TIME to show cuePreference
            bool runCpNow = false;
            if (coreManagerCurrentIndex == 4 && !_cuePreferenceDoneAfterIndex3)
            {
                runCpNow = true;
                _cuePreferenceDoneAfterIndex3 = true;
            }
            else if (coreManagerCurrentIndex == 9 && !_cuePreferenceDoneAfterIndex8)
            {
                runCpNow = true;
                _cuePreferenceDoneAfterIndex8 = true;
            }
            else if (coreManagerCurrentIndex == 14 && !_cuePreferenceDoneAfterIndex13)
            {
                runCpNow = true;
                _cuePreferenceDoneAfterIndex13 = true;
            }

            if (runCpNow)
            {
                //Debug.Log($"[QuestionnaireManager] Triggering Cue Preference (CP.json) found at sequence index {_currentSequenceIndex}. CoreManager index is {coreManagerCurrentIndex}.");
                questionnaireController.StartQuestionnaire(nextFile, _sessionParticipantID, _currentCondition , _currentTask);
            }
            else
            {
                //Debug.Log($"[QuestionnaireManager] Skipping Cue Preference (CP.json) found at sequence index {_currentSequenceIndex}. CoreManager index is {coreManagerCurrentIndex}. Required index 4/9/14 or already done.");
                _currentSequenceIndex++; 
                StartNextQuestionnaire(); 
                return;
            }
        }
        else
        {
            questionnaireController.StartQuestionnaire(nextFile, _sessionParticipantID, _currentCondition, _currentTask);
        }
    }

    private void HandleQuestionnaireCompleted()
    {
        _currentSequenceIndex++;
        StartNextQuestionnaire();
    }

    // Helper function to call CoreManager's LoadNextScene safely
    private void ProceedToNextCoreScene()
    {
        if (SceneManager.GetActiveScene().name == "SSQ-Scene")
        {
            ssqPanelControllerEnding.EnableSSQEnding();

        }
        else if (CoreManager.Instance != null)
        {
            CoreManager.Instance.LoadNextScene();
        }
        else
        {
            Debug.LogError("[QuestionnaireManager] CoreManager not found, cannot proceed to next scene automatically.");
        }
    }
}