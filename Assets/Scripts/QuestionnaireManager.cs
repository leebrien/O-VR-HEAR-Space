using UnityEngine;
using System.Collections.Generic;

public class QuestionnaireManager : MonoBehaviour
{
    [SerializeField] private List<TextAsset> questionnaireSequence;
    [SerializeField] private QuestionnaireController questionnaireController;

    private int currentQuestionnaireIndex = 0;

    void Start()
    {
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
            Debug.Log($"--- Starting Questionnaire {currentQuestionnaireIndex + 1} ---");
            TextAsset nextFile = questionnaireSequence[currentQuestionnaireIndex];
            questionnaireController.StartQuestionnaire(nextFile);
        }
        else
        {
            Debug.Log("--- Study Completed! All questionnaires are finished. ---");
        }
    }

    private void HandleQuestionnaireCompleted()
    {
        Debug.Log("--- Questionnaire Completed, moving to next. ---");
        currentQuestionnaireIndex++;
        StartNextQuestionnaire();
    }
}