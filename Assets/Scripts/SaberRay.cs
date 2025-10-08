using UnityEngine;

public class SaberRay : MonoBehaviour
{
    public Transform rayOrigin; // Controller tip
    public float maxLength = 4.5f;
    public LayerMask interactLayer;

    private LineRenderer _lineRenderer;
    private bool _saberActive = false;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    void Update()
    {
        if (!_saberActive) return;

        Vector3 start = rayOrigin.position;
        Vector3 direction = rayOrigin.forward;
        Vector3 endPoint;

        if (Physics.Raycast(start, direction, out RaycastHit hit, maxLength, interactLayer))
        {
            endPoint = hit.point;
            // You can trigger hit logic here if needed
        }
        else
        {
            endPoint = start + direction * maxLength;
        }

        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, endPoint);
    }

    public void SetActive(bool active)
    {
        _saberActive = active;
        _lineRenderer.enabled = active;
    }
}