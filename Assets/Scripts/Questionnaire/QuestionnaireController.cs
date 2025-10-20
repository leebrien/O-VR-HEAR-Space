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
    [SerializeField] private GameObject multipleChoiceViewGO;
    [SerializeField] private GameObject cuePreferenceViewGO;

    // Reference to Scripts
    private SliderView sliderView;
    private ToggleGroupView radioView;
    private ToggleGroupView multipleChoiceView;
    private ToggleGroupView cuePreferenceView;

    [SerializeField] private Button forwardButton;
    [SerializeField] private TextMeshProUGUI forwardButtonText;
    [SerializeField] private Button backButton;

    private bool isSubmitting = false;
    private string currentParticipantID;

    void Awake()
    {
        sliderView = sliderViewGO.GetComponent<SliderView>();
        radioView = radioViewGO.GetComponent<ToggleGroupView>();
        multipleChoiceView = multipleChoiceViewGO.GetComponent<ToggleGroupView>();
        cuePreferenceView = cuePreferenceViewGO.GetComponent<ToggleGroupView>();

        ToggleGroupView.OnSelectionMade += HandleSelectionMade;
    }

    private void OnDestroy()
    {
        ToggleGroupView.OnSelectionMade -= HandleSelectionMade;
    }

    private void HandleSelectionMade()
    {
        forwardButton.interactable = true;
    }

    public void StartQuestionnaire(TextAsset jsonFile, string participantID)
    {
        currentParticipantID = participantID;
        model.InitializeWithData(jsonFile);
        questionnairePanel.SetActive(true);
        isSubmitting = false;

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

    private void DeactivateAllViews()
    {
        sliderViewGO.SetActive(false);
        radioViewGO.SetActive(false);
        multipleChoiceViewGO.SetActive(false);
        cuePreferenceViewGO.SetActive(false);
    }

    private void DisplayCurrentQuestion()
    {
        QuestionData qData = model.GetCurrentQuestion();

        if (qData == null) return; // guard clause

        DeactivateAllViews();

        if (qData.type == "slider")
        {
            sliderViewGO.SetActive(true);
            forwardButton.interactable = true;
            questionnaireUIView.UpdateView(model.QuestionnaireName, qData.text, model.currentQuestionIndex, model.GetTotalQuestions());
            sliderView.UpdateView(qData.minValue, qData.maxValue, qData.lowLabel, qData.highLabel);
        }
        else if (qData.type == "radio")
        {
            radioViewGO.SetActive(true);
            forwardButton.interactable = false;
            questionnaireUIView.UpdateView(model.QuestionnaireName, "", model.currentQuestionIndex, model.GetTotalQuestions());
            radioView.UpdateView(qData.lowLabel, qData.highLabel, qData.steps);
        }
        else if (qData.type == "multipleChoice")
        {
            multipleChoiceViewGO.SetActive(true);
            forwardButton.interactable = false;
            questionnaireUIView.UpdateView(model.QuestionnaireName, qData.text, model.currentQuestionIndex, model.GetTotalQuestions());
            multipleChoiceView.ResetView();
        }
        else if (qData.type == "cuePreference")
        {
            cuePreferenceViewGO.SetActive(true);
            forwardButton.interactable = false;
            questionnaireUIView.UpdateView(model.QuestionnaireName, qData.text, model.currentQuestionIndex, model.GetTotalQuestions());
            cuePreferenceView.ResetView();
        }

        // Backtracking logic
        if (model.sessionResponses.ContainsKey(qData.id))
        {
            float savedScore = model.sessionResponses[qData.id];
            forwardButton.interactable = true; // Re-enable if already answered

            if (qData.type == "slider") { sliderView.SetScore(savedScore); }
            else if (qData.type == "radio") { radioView.SetScore(savedScore); }
            else if (qData.type == "multipleChoice") { multipleChoiceView.SetScore(savedScore); }
            else if (qData.type == "cuePreference") { cuePreferenceView.SetScore(savedScore); }
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
        if (isSubmitting) return; // gate

        QuestionData qData = model.GetCurrentQuestion();
        if (qData == null) return;

        // --- Uses float score ---
        float score = GetCurrentScore(qData);
        if (score != -1f)
        {
            model.LogResponse(qData.id, score);
        }

        // Submit questionnaire
        if (model.currentQuestionIndex == model.GetTotalQuestions() - 1)
        {
            isSubmitting = true;
            model.SubmitData(currentParticipantID);
            questionnairePanel.SetActive(false);
            Debug.Log($"Questionnaire '{model.QuestionnaireName}' finished for {currentParticipantID}.");
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
        if (qData.type == "slider") { return sliderView.GetScore(); }
        else if (qData.type == "radio") { return radioView.GetScore(); }
        else if (qData.type == "multipleChoice") { return multipleChoiceView.GetScore(); }
        else if (qData.type == "cuePreference") { return cuePreferenceView.GetScore(); }
        return -1f;
    }
}