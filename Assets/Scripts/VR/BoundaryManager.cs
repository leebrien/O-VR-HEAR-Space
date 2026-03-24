using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    private Transform _playerTransform;
    public Renderer[] wallRenderers;
    public Collider[] wallColliders;

    public Color fadeColor = Color.red;
    public float fadeDistance = 1.0f;

    private Color[] _initialColors;

    void Start()
    {
        _playerTransform = CoreManager.Instance.GetCenterEyeAnchor();
        _initialColors = new Color[wallRenderers.Length];
        for (int i = 0; i < wallRenderers.Length; i++)
        {
            _initialColors[i] = wallRenderers[i].material.color;
        }
    }

    void Update()
    {
        if (!_playerTransform) return;
        if (wallRenderers.Length != wallColliders.Length)
        {
            return;
        }

        for (int i = 0; i < wallRenderers.Length; i++)
        {
            // Calculate the closest point on the collider to the player
            Vector3 closestPoint = wallColliders[i].ClosestPoint(_playerTransform.position);

            // Calculate the distance from the player to that closest point
            float distance = Vector3.Distance(_playerTransform.position, closestPoint);

            float colorFactor = Mathf.Clamp01(distance / fadeDistance);

            wallRenderers[i].material.color = Color.Lerp(fadeColor, _initialColors[i], colorFactor);
        }
    }
}