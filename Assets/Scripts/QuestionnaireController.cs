using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionnaireController : MonoBehaviour
{
    [Header("MVC Components")]
    [SerializeField] private QuestionnaireModel model;
    [SerializeField] private QuestionnairePanelView view;

    [Header("Navigation UI")]
    [SerializeField] private Button forwardButton;
    [SerializeField] private TextMeshProUGUI forwardButtonText;
    [SerializeField] private Button backButton;

    void Start()
    {
        // Use this aside from using the OnClick() function in the button inspector. This is to prevent the asset from breaking
        // when something is being changed.
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
        Debug.Log($"Controller displaying question index: {model.currentQuestionIndex}");

        view.UpdateView(
            model.QuestionnaireName,
            qData.text,
            qData.minValue,
            qData.maxValue,
            qData.lowLabel,
            qData.highLabel,
            model.currentQuestionIndex,
            model.GetTotalQuestions()
        );

        // Check if question is answered. If so, set the slider value.
        if (model.sessionResponses.ContainsKey(qData.id))
        {
            float savedScore = model.sessionResponses[qData.id];
            view.SetScore(savedScore);
            Debug.Log($"Found saved score for {qData.id}: {savedScore}. Setting slider.");
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

        if (qData == null)
        {
            Debug.LogError("Cannot process submission. QuestionData is null.");
            return;
        }

        float score = view.GetScore();
        model.LogResponse(qData.id, score);

        if (model.currentQuestionIndex == model.GetTotalQuestions() - 1)
        {
            model.SubmitData();
            view.gameObject.SetActive(false);
            Debug.Log("Questionnaire finished and data submitted.");
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
            model.LogResponse(qData.id, view.GetScore());
        }

        if (model.currentQuestionIndex > 0)
        {
            model.RetreatQuestion();
            DisplayCurrentQuestion();
        }
    }
}