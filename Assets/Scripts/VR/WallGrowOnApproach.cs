using UnityEngine;

public class WallGrowOnApproach : MonoBehaviour
{
    public Color glowColor = Color.red;
    public float detectionRadius = 1f;
    public float glowIntensity = 1.5f;
    
    private Renderer wallRenderer;
    private Material wallMaterial;
    private Color originalColor;
    private bool isGlowing=false;

    void Start()
    {
        wallRenderer = GetComponent<Renderer>();
        if (wallRenderer)
        {
            wallMaterial = wallRenderer.material;
            originalColor = wallMaterial.color;
        }
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player || wallMaterial == null) return;
        
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance < detectionRadius && !isGlowing)
        {
            // Glow red
            wallMaterial.color = glowColor;
            wallMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
            DynamicGI.SetEmissive(wallRenderer, glowColor * glowIntensity); // optional for baked lighting
            isGlowing = true;
        }
        else if (distance >= detectionRadius && isGlowing)
        {
            // Revert
            wallMaterial.color = originalColor;
            wallMaterial.SetColor("_EmissionColor", Color.black);
            DynamicGI.SetEmissive(wallRenderer, Color.black);
            isGlowing = false;
        }
    }
}
