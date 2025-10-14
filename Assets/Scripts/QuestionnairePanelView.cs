using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionnairePanelView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionnaireNameText;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI lowLabelText;
    [SerializeField] private TextMeshProUGUI highLabelText;
    [SerializeField] private TextMeshProUGUI questionIndexText;


    // Configuration for the questionnaire panel's UI. 
    public void UpdateView(
        string qName,
        string qText,
        float min,
        float max,
        string lowL,
        string highL,
        int currentIndex,
        int totalQuestions)
    {
        questionnaireNameText.text = qName;
        questionText.text = qText;
        lowLabelText.text = lowL;
        highLabelText.text = highL;

        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = true;
        slider.value = min; // Default to min value

        questionIndexText.text = $"{currentIndex + 1} / {totalQuestions}";
    }

    public float GetScore()
    {
        return Mathf.Round(slider.value);
    }

    // Set slider value for back navigation
    public void SetScore(float score)
    {
        slider.value = score;
    }
}