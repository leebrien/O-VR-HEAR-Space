using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class QuestionnaireController : MonoBehaviour
{
    public static event Action OnQuestionnaireCompleted;

    [SerializeField] private QuestionnaireModel model;

    [SerializeField] private GameObject questionnairePanel;

    [SerializeField] private QuestionnaireView questionnaireUIView;

    // Reference to Game Objects
    [SerializeField] private GameObject sliderViewGO;
    [SerializeField] private GameObject radioViewGO;

    // Reference to Scripts
    private SliderView sliderView;
    private RadioView radioView;

    [SerializeField] private Button forwardButton;
    [SerializeField] private TextMeshProUGUI forwardButtonText;
    [SerializeField] private Button backButton;

    private bool isSubmitting = false;

    void Awake()
    {
        sliderView = sliderViewGO.GetComponent<SliderView>();
        radioView = radioViewGO.GetComponent<RadioView>();

        RadioView.OnRadioSelectionMade += HandleRadioSelectionMade;
    }

    private void OnDestroy()
    {
        RadioView.OnRadioSelectionMade -= HandleRadioSelectionMade;
    }

    private void HandleRadioSelectionMade()
    {
        // As soon as a selection is made, make the button clickable
        forwardButton.interactable = true;

    }
    public void StartQuestionnaire(TextAsset jsonFile)
    {

        model.InitializeWithData(jsonFile);
        questionnairePanel.SetActive(true);
        isSubmitting = false;

    // This is needed for transitioning to the next questionnaire.
    forwardButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();

        forwardButton.onClick.AddListener(OnForwardClicked);
        backButton.onClick.AddListener(OnBackClicked);

        if (model.GetTotalQuestions() > 0)
        {
            DisplayCurrentQuestion();
        }
        else
        {
            Debug.LogError("No questionnaires loaded.");
        }
    }

    private void DisplayCurrentQuestion()
    {
        QuestionData qData = model.GetCurrentQuestion();
        
        // guard clause
        if (qData == null) return;

        // This checks which answer type UI to display
        // If you made your own UI, you have to manually insert it here.

        if (qData.type == "slider")
        {
            sliderViewGO.SetActive(true);
            radioViewGO.SetActive(false);

            forwardButton.interactable = true;

            // 1. Update Main Questionnaire View
            questionnaireUIView.UpdateView(
                model.QuestionnaireName,
                qData.text,
                model.currentQuestionIndex,
                model.GetTotalQuestions()
            );

            // 2. Update the Slider View
            sliderView.UpdateView(
                qData.minValue,
                qData.maxValue,
                qData.lowLabel,
                qData.highLabel
            );
        }
        else if (qData.type == "radio")
        {
            sliderViewGO.SetActive(false);
            radioViewGO.SetActive(true);

            // Disable at first to prevent null responses
            forwardButton.interactable = false;

            // 1. Update the Main Questionnaire View
            questionnaireUIView.UpdateView(
                model.QuestionnaireName,
                "", // UEQ-S doesn't have questions
                model.currentQuestionIndex,
                model.GetTotalQuestions()
            );

            // 2. Update the Slider View
            radioView.UpdateView(
                qData.lowLabel,
                qData.highLabel,
                qData.steps
            );
        }

        // This is for backtracking and reloading previous answered questions
        // If you made your own UI, you have to manually insert it here.
        if (model.sessionResponses.ContainsKey(qData.id))
        {
            float savedScore = model.sessionResponses[qData.id];
            if (qData.type == "slider")
            {
                sliderView.SetScore(savedScore);
            }
            else if (qData.type == "radio")
            {
                radioView.SetScore(savedScore);
                
                // Reenable
                forwardButton.interactable = true;
            }
        }

        UpdateNavigationButtons();
    }

    // Set 'Next' button to 'Submit' at the final question.
    private void UpdateNavigationButtons()
    {
        int total = model.GetTotalQuestions();
        int index = model.currentQuestionIndex;
        backButton.gameObject.SetActive(index > 0);
        forwardButtonText.text = (index == total - 1) ? "Submit" : "Next";
    }

    public void OnForwardClicked()
    {
        // gate
        if (isSubmitting) return;

        QuestionData qData = model.GetCurrentQuestion();
        if (qData == null) return;

        float score = GetCurrentScore(qData);
        if (score != -1f)
        {
            model.LogResponse(qData.id, score);
        }

        // Submit questionnaire
        if (model.currentQuestionIndex == model.GetTotalQuestions() - 1)
        {
            // Set the flag so this block can't run again
            isSubmitting = true;

            model.SubmitData();

            questionnairePanel.SetActive(false);

            Debug.Log("Questionnaire finished.");
            OnQuestionnaireCompleted?.Invoke();
            return;
        }

        model.AdvanceQuestion();
        DisplayCurrentQuestion();
    }

    public void OnBackClicked()
    {
        QuestionData qData = model.GetCurrentQuestion();
        if (qData != null)
        {
            float score = GetCurrentScore(qData);
            if (score != -1f)
            {
                model.LogResponse(qData.id, score);
            }
        }

        if (model.currentQuestionIndex > 0)
        {
            model.RetreatQuestion();
            DisplayCurrentQuestion();
        }
    }

    // If you made your own UI, you have to manually insert it here.
    private float GetCurrentScore(QuestionData qData)
    {
        if (qData.type == "slider")
        {
            return sliderView.GetScore();
        }
        else if (qData.type == "radio")
        {
            return radioView.GetScore();
        }
        return -1f;
    }
}