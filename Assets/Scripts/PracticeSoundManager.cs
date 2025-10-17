using System;
using System.Collections;
using Unity.Multiplayer.Center.Common;
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
    private Renderer _pointingRenderer;
    private Renderer _grabbingRenderer;
    private bool _firstLog;

    public void Start()
    {
        _firstLog = CoreManager.Instance.GetFirstLog();
        if (_firstLog)
        {
           
        }
        if (pointingObject) _pointingRenderer = pointingObject.GetComponent<Renderer>();
        if (grabbingObject) _grabbingRenderer = grabbingObject.GetComponent<Renderer>();
    }

    public TrackingSwitcher GetTrackingSwitcher()
    {
        return trackingSwitcher ?? null;
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

    public bool GetLoggingStatus()
    {
        return _firstLog;
    }

    private void GenerateObjectPosition(ref Vector3 objectPos, int taskType)
    {
        if (taskType == 1)
        {
            objectPos.z += 2.5f;
            objectPos.x += Random.Range(-3.75f, 3.75f);
        }
        else if (taskType == 2 || taskType == 3)
        {
            int direction = Random.Range(0, 8);
           // float offset = Random.Range(1.75f, 3.75f);
            float[] allowedoffsets = { 0.75f, 1f, 1.25f };
            float offset = allowedoffsets[Random.Range(0, allowedoffsets.Length)];

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
        
        SetMeshMaterial(material);
        if (recentTaskType == 1)
        {
            saberManager.DisableSabers();
            trackingSwitcher.SwitchToHandsOnly();
        }
        StartCoroutine(CompleteTaskAfterDelay(0.8f));
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
        Renderer activeRenderer = pointingObject.activeSelf ? _pointingRenderer :
            (grabbingObject.activeSelf ? _grabbingRenderer : null);

        if (activeRenderer)
        {
            activeRenderer.material = material;
        }
    }
    
}