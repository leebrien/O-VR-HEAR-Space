using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuestionnaireManager : MonoBehaviour
{
    [Header("Questionnaire Files")]
    [SerializeField] private List<TextAsset> questionnaireSequence;

    [Header("References")]
    [SerializeField] private QuestionnaireController questionnaireController;
    [Header("References")]

    private int _currentSequenceIndex; // Tracks progress through the questionnaireSequence list
    private string _sessionParticipantID;
    private string _currentCondition;
    private int _currentTask;
    private int _currentSceneIndex;

    // Flags to ensure Cue Preference runs only once per task pair trigger point
    private bool _cuePreferenceDoneAfterIndex5; // Triggered when CoreManager index becomes 4
    private bool _cuePreferenceDoneAfterIndex12; // Triggered when CoreManager index becomes 11
    private bool _cuePreferenceDoneAfterIndex19; // Triggered when CoreManager index becomes 18
    

    void Start()
    {
        // Get Participant ID from CoreManager
        if (CoreManager.Instance != null)
        {
            _sessionParticipantID = CoreManager.Instance.GetSessionParticipantID();
            _currentCondition = CoreManager.Instance.GetCurrentCondition();
            _currentTask = CoreManager.Instance.GetCurrentTask();
            _currentSceneIndex = CoreManager.Instance.GetCurrentSceneIndex();
            
            
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
            if (coreManagerCurrentIndex == 6 && !_cuePreferenceDoneAfterIndex5)
            {
                runCpNow = true;
                _cuePreferenceDoneAfterIndex5 = true;
            }
            else if (coreManagerCurrentIndex == 13 && !_cuePreferenceDoneAfterIndex12)
            {
                runCpNow = true;
                _cuePreferenceDoneAfterIndex12 = true;
            }
            else if (coreManagerCurrentIndex == 19 && !_cuePreferenceDoneAfterIndex19)
            {
                runCpNow = true;
                _cuePreferenceDoneAfterIndex19 = true;
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