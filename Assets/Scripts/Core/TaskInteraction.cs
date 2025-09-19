using UnityEngine;
using TMPro;

public class TaskInteraction : MonoBehaviour
{
    public Material successMaterial;
    public TaskPanel taskPanel;
    public GameObject[] environmentObjects;
    public GameObject audioObject;
    public GameObject proceedButton;
    public TextMeshProUGUI displayText;
    public TextMeshProUGUI interactionStatusText;
    public Rigidbody audioRB;

    private Vector3 _soundPosition;

    public void OnSuccess()
    {
        Debug.Log("HIT HIT HIT!");

        // Stop sound
        if (taskPanel && SoundManager.Instance && !taskPanel.panelToHide.activeSelf)
        {
            SoundManager.Instance.StopSound();
        }

        // Show the panel again
        if (taskPanel && !taskPanel.panelToHide.activeSelf)
        {
            taskPanel.ShowPanel();

            if (proceedButton)
            {
                proceedButton.SetActive(true);
            }

            if (displayText)
            {
                displayText.gameObject.SetActive(true);

                var currentCondition = CoreManager.Instance.currentCondition;
                var currentTask = CoreManager.Instance.currentTask;

                var taskType = currentTask switch
                {
                    1 => "Pointing",
                    2 => "Grabbing",
                    3 => "Complex Grabbing",
                    _ => "Unknown Task"
                };

                var cueType = currentCondition switch
                {
                    "PC" => "Personalized Cue",
                    "GC" => "Generic Cue",
                      _  => "Unknown Cue"
                };
                
                displayText.text = $"TASK {currentTask}: {taskType} ({cueType})";
                interactionStatusText.text = "Status: Task Completed! You may proceed.";
                interactionStatusText.gameObject.SetActive(true);
            }

        }
    }
    public void revertAudioPosition()
    {   
        if (audioRB != null)
        {
            audioRB.MovePosition(SoundManager.Instance._soundPosition);
            Debug.Log("Moved sound source to: " + _soundPosition);
        }
        
    }

    public void SetMeshMaterial(Material newMaterial)
    {
        foreach (GameObject envObject in environmentObjects)
        {
            Renderer renderer = envObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = newMaterial;
            }
        }
    }

}