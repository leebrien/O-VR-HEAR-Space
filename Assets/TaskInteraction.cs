using UnityEngine;
using TMPro;

public class TaskInteraction : MonoBehaviour
{
    public Material successMaterial;
    public GameObject[] environmentObjects;
    public TaskPanel taskPanel;

    public GameObject proceedButton;
    public TextMeshProUGUI displayText;

    //public Material skyboxMaterial;

    public void OnSuccess()
    {
        Debug.Log("HIT HIT HIT!");

        
        // Change the environment color
        foreach (GameObject envObject in environmentObjects)
        {
            Renderer renderer = envObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = successMaterial;
            }
        }

        /*
        // Change the skybox
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }
        */

        // Stop sound
        if (taskPanel != null && taskPanel.soundManager != null)
        {
            taskPanel.soundManager.StopSound();
        }

        // Show the panel again
        if (taskPanel != null)
        {
            taskPanel.ShowPanel();
        }

        if (proceedButton != null)
        {
            proceedButton.SetActive(true);
            // Decrease the button's Y position by 20
            RectTransform buttonTransform = proceedButton.GetComponent<RectTransform>();
            if (buttonTransform != null)
            {
                Vector2 newPos = buttonTransform.anchoredPosition;
                newPos.y -= 20;
                buttonTransform.anchoredPosition = newPos;
            }
        }

        if (displayText != null)
        {
            displayText.gameObject.SetActive(true);
            displayText.text = "Task Completed";

            // Increase the display text's Y position by 20
            RectTransform textTransform = displayText.GetComponent<RectTransform>();
            if (textTransform != null)
            {
                Vector2 newPos = textTransform.anchoredPosition;
                newPos.y += 20;
                textTransform.anchoredPosition = newPos;
            }

            // Set the display text's font size to 20
            displayText.fontSize = 10;
        }
    }
}