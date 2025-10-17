using UnityEngine;

[ExecuteAlways]
public class DynamicGrid : MonoBehaviour
{
    private static readonly int CenterPos = Shader.PropertyToID("_CenterPos");
    public Material gridMaterial;
    public Transform target; 
    public float yOffset = 0f; 

    void Update()
    {
        if (gridMaterial is null || target is null)
            return;

        Vector3 pos = target.position;
        pos.y = yOffset; // keep it flat on ground
        gridMaterial.SetVector(CenterPos, new Vector4(pos.x, pos.y, pos.z, 0));
    }
}

