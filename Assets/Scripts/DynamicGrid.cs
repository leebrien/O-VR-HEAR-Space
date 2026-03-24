using System;
using UnityEngine;

[ExecuteAlways]
public class DynamicGrid : MonoBehaviour
{
    private static readonly int CenterPos = Shader.PropertyToID("_CenterPos");
    public Material gridMaterial;
    private Transform _target; 
    public float yOffset = 0f;

    private void Start()
    {
        if (_target == null && CoreManager.Instance != null)
        {
            _target = CoreManager.Instance.GetCenterEyeAnchor();
        }
    }

    void Update()
    {
        if (!Application.isPlaying) return;
        if (gridMaterial is null || _target is null)
            return;

        Vector3 pos = _target.position;
        pos.y = yOffset; // keep it flat on ground
        gridMaterial.SetVector(CenterPos, new Vector4(pos.x, pos.y, pos.z, 0));
    }
}

