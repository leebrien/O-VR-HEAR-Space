using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RadioView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lowLabelText;
    [SerializeField] private TextMeshProUGUI highLabelText;

    //Assign all your radio button toggles here in order from left to right
    [SerializeField] private List<Toggle> radioToggles;

    private ToggleGroup toggleGroup;

    void Awake()
    {
        toggleGroup = GetComponentInParent<ToggleGroup>();
        if (toggleGroup == null)
        {
            Debug.LogError("No ToggleGroup found for the RadioView!");
        }
    }

    public void UpdateView(string lowLabel, string highLabel, int steps)
    {
        lowLabelText.text = lowLabel;
        highLabelText.text = highLabel;

        // Activate the correct number of toggles based on 'steps'
        for (int i = 0; i < radioToggles.Count; i++)
        {
            radioToggles[i].gameObject.SetActive(i < steps);
        }

        // Deselect all toggles to reset the view for the new question
        toggleGroup.SetAllTogglesOff();
    }

    public float GetScore()
    {   
        // Checks which toggle is on
        for (int i = 0; i < radioToggles.Count; i++)
        {
            if (radioToggles[i].isOn)
            {
                return i + 1; // Return 1 for the first button, 2 for the second, etc.
            }
        }
        return -1f; // no toggle selected
    }

    public void SetScore(float score)
    {
        int index = Mathf.RoundToInt(score) - 1;

        if (index >= 0 && index < radioToggles.Count)
        {
            radioToggles[index].isOn = true;
        }
    }
}