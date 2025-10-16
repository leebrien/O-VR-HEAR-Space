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

    void Awake()
    {
        sliderView = sliderViewGO.GetComponent<SliderView>();
        radioView = radioViewGO.GetComponent<RadioView>();

        // for debugging 
        if (questionnairePanel == null) Debug.LogError("Questionnaire Panel GameObject is not assigned in the Inspector!", this.gameObject);
        if (questionnaireUIView == null) Debug.LogError("QuestionnaireUIView is not assigned in the Inspector!", this.gameObject);
        if (sliderView == null) Debug.LogError("The 'sliderViewGO' GameObject is missing the 'SliderView' script component!", this.gameObject);
        if (radioView == null) Debug.LogError("The 'radioViewGO' GameObject is missing the 'RadioView' script component!", this.gameObject);
    }

    public void StartQuestionnaire(TextAsset jsonFile)
    {
        model.InitializeWithData(jsonFile);
        questionnairePanel.SetActive(true);

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
            Debug.LogError("No questions loaded! Check JSON file.");
        }
    }

    private void DisplayCurrentQuestion()
    {
        QuestionData qData = model.GetCurrentQuestion();
        if (qData == null) return;

        Debug.Log($"Controller displaying question index: {model.currentQuestionIndex}");

        if (qData.type == "slider")
        {
            sliderViewGO.SetActive(true);
            radioViewGO.SetActive(false);

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

        // Check for a saved response and update the CORRECT view
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
            }
        }

        UpdateNavigationButtons();
    }

    private void UpdateNavigationButtons()
    {
        int total = model.GetTotalQuestions();
        int index = model.currentQuestionIndex;
        backButton.gameObject.SetActive(index > 0);
        forwardButtonText.text = (index == total - 1) ? "Submit" : "Next";
    }

    public void OnForwardClicked()
    {
        QuestionData qData = model.GetCurrentQuestion();
        if (qData == null) return;

        float score = GetCurrentScore(qData);
        if (score != -1f)
        {
            model.LogResponse(qData.id, score);
        }

        if (model.currentQuestionIndex == model.GetTotalQuestions() - 1)
        {
            model.SubmitData();
            sliderViewGO.SetActive(false);
            radioViewGO.SetActive(false);
            Debug.Log("Questionnaire finished and data submitted.");

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