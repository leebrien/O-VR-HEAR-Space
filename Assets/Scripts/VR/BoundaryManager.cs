using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    public Transform playerTransform;
    public Renderer[] wallRenderers;
    public Collider[] wallColliders;

    public Color fadeColor = Color.red;
    public float fadeDistance = 1.0f;

    private Color[] _initialColors;

    void Start()
    {
        _initialColors = new Color[wallRenderers.Length];
        for (int i = 0; i < wallRenderers.Length; i++)
        {
            _initialColors[i] = wallRenderers[i].material.color;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;
        if (wallRenderers.Length != wallColliders.Length)
        {
            Debug.LogError("Wall Renderers and Wall Colliders arrays must be the same size!");
            return;
        }

        for (int i = 0; i < wallRenderers.Length; i++)
        {
            // Calculate the closest point on the collider to the player
            Vector3 closestPoint = wallColliders[i].ClosestPoint(playerTransform.position);

            // Calculate the distance from the player to that closest point
            float distance = Vector3.Distance(playerTransform.position, closestPoint);

            float colorFactor = Mathf.Clamp01(distance / fadeDistance);

            wallRenderers[i].material.color = Color.Lerp(fadeColor, _initialColors[i], colorFactor);
        }
    }
}