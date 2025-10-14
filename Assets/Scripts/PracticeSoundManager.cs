using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PracticeSoundManager : MonoBehaviour
{
    public Transform user; // Assign to center eye anchor

    public GameObject pointingObject; // Object for pointing
    public GameObject grabbingObject; // Object for grabbing
    public GameObject lobbyPanel;

    private readonly Vector3 _roomSize = new Vector3(8.0f, 2.5f, 8.0f);

    public PracticeUIManagement uiManager;
    public SaberManager saberManager;
    public TrackingSwitcher trackingSwitcher;
    
    private int recentTaskType = 0;

    public void Start()
    {
        trackingSwitcher.SwitchToHandsOnly();
    }

    public void PlayObject(int taskType)
    {
        pointingObject?.SetActive(false);
        grabbingObject?.SetActive(false);

        GameObject activeObject;
        if (taskType == 1)
        {
            trackingSwitcher.SwitchToControllersOnly();
            saberManager.EnableSabers();
            activeObject = pointingObject;
            recentTaskType = 1;
        }
        else if (taskType == 2)
        {
            activeObject = grabbingObject;
            recentTaskType = 2;
        }
        else
        {
            Debug.LogWarning("Invalid task type provided.");
            return;
        }

        if (user == null || activeObject == null)
        {
            Debug.LogWarning("Required components not assigned in the Inspector.");
            return;
        }

        Vector3 objectPosition = user.position;
        GenerateObjectPosition(ref objectPosition, taskType);

        activeObject.transform.position = objectPosition;
        activeObject.SetActive(true);
    }

    public void StopObject()
    {
        pointingObject?.SetActive(false);
        grabbingObject?.SetActive(false);
    }

    private void GenerateObjectPosition(ref Vector3 objectPos, int taskType)
    {
        if (taskType == 1)
        {
            objectPos.z += 3.5f;
            objectPos.x += Random.Range(-3.75f, 3.75f);
        }
        else if (taskType == 2 || taskType == 3)
        {
            int direction = Random.Range(0, 8);
            float offset = Random.Range(1.75f, 3.75f);

            switch (direction)
            {
                case 0: objectPos.z += offset; break;
                case 1: objectPos.z -= offset; break;
                case 2: objectPos.x += offset; break;
                case 3: objectPos.x -= offset; break;
                case 4: objectPos.z += offset; objectPos.x += offset; break;
                case 5: objectPos.z += offset; objectPos.x -= offset; break;
                case 6: objectPos.z -= offset; objectPos.x += offset; break;
                case 7: objectPos.z -= offset; objectPos.x -= offset; break;
            }
        }

        objectPos.y = Random.Range(1, user.position.y + 0.3f);
        objectPos.x = Mathf.Clamp(objectPos.x, -_roomSize.x / 2, _roomSize.x / 2);
        objectPos.z = Mathf.Clamp(objectPos.z, -_roomSize.z / 2, _roomSize.z / 2);
    }

    public void OnSuccess(Material material)
    {
        Debug.Log("HIT HIT HIT!");

        StartCoroutine(CompleteTaskAfterDelay(2f));
        if (recentTaskType == 1)
        {
            saberManager.DisableSabers();
            trackingSwitcher.SwitchToHandsOnly();
        }
        
        SetMeshMaterial(material);
    }

    // delay before compeletion
    private IEnumerator CompleteTaskAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (uiManager)
        {
            uiManager.OnTaskComplete();
        }
    }

    public void SetMeshMaterial(Material material)
    {
        GameObject activeObject = pointingObject.activeSelf ? pointingObject : (grabbingObject.activeSelf ? grabbingObject : null);

        if (activeObject != null)
        {
            // change material to green
            Renderer objectRenderer = activeObject.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                objectRenderer.material = material;
            }
        }
    }
    
}