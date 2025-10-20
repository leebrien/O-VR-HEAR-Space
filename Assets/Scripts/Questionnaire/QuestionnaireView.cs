using UnityEngine;
using TMPro;

public class QuestionnaireView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionnaireNameText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI questionIndexText;

    public void UpdateView(string qName, string qText, int currentIndex, int totalQuestions)
    {
        if (questionnaireNameText != null)
        {
            questionnaireNameText.text = qName;
        }

        if (questionText != null)
        {
            questionText.text = qText;
        }

        if (questionIndexText != null)
        {
            questionIndexText.text = $"{currentIndex + 1} / {totalQuestions}";
        }
    }
}