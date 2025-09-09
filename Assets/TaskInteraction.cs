using UnityEngine;
using TMPro;

public class TaskInteraction : MonoBehaviour
{
    public Material successMaterial;
    //public GameObject[] environmentObjects;
    public TaskPanel taskPanel;

    public GameObject proceedButton;
    public TextMeshProUGUI displayText;

    public void OnSuccess()
    {
        Debug.Log("HIT HIT HIT!");

        // Change the environment color
        /*foreach (GameObject envObject in environmentObjects)
        {
            Renderer renderer = envObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = successMaterial;
            }
        }
        */

        // Stop sound
        if (taskPanel != null && taskPanel.soundManager != null && !taskPanel.panelToHide.activeSelf)
        {
            taskPanel.soundManager.StopSound();
        }

        // Show the panel again
        if (taskPanel != null && !taskPanel.panelToHide.activeSelf)
        {
            taskPanel.ShowPanel();

            if (proceedButton != null)
            {
                proceedButton.SetActive(true);
            }

            if (displayText != null)
            {
                displayText.gameObject.SetActive(true);

                string currentCondition = CoreManager.Instance.currentCondition;
                int currentTask = CoreManager.Instance.currentTask;
                displayText.text = $"Task {currentTask} ({currentCondition}) Completed";

                RectTransform textTransform = displayText.GetComponent<RectTransform>();
                if (textTransform != null)
                {
                    Vector2 newPos = textTransform.anchoredPosition;
                    newPos.y += 10;
                    textTransform.anchoredPosition = newPos;
                }
                displayText.fontSize = 10;
            }

        }
    }
}