using UnityEngine;
using TMPro;

/// <summary>
/// Manages the shared UI elements of the questionnaire panel, such as the
/// main title, question text, and progress index.
/// This script should be placed on the main Questionnaire GameObject.
/// </summary>
public class QuestionnaireView : MonoBehaviour
{
    [Header("Shared UI References")]
    [SerializeField] private TextMeshProUGUI questionnaireNameText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI questionIndexText;

    /// <summary>
    /// Updates all the shared UI elements with new data.
    /// </summary>
    public void UpdateView(string qName, string qText, int currentIndex, int totalQuestions)
    {
        // Only show the questionnaire name if it's provided
        if (questionnaireNameText != null)
        {
            questionnaireNameText.text = qName;
        }

        // The main text of the question (e.g., "How mentally demanding...")
        if (questionText != null)
        {
            questionText.text = qText;
        }

        // The progress text (e.g., "1 / 6")
        if (questionIndexText != null)
        {
            questionIndexText.text = $"{currentIndex + 1} / {totalQuestions}";
        }
    }
}