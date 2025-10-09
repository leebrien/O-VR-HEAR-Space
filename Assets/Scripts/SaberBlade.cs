using UnityEngine;
using UnityEngine.XR;

public class SaberBlade : MonoBehaviour
{
    public string targetTag = "CueSource";
    public Material greenMaterial;
    public Material transparentMaterial;
    public OVRInput.Controller controller; // Left or Right controller
    public LayerMask targetLayer; // Layer mask to filter collisions
    public TaskInteraction taskInteraction; // Must have OnSuccess() and SetMeshMaterial(Material)

    private bool isColliding = false;
    private bool wasTriggerPressed = false;

    void Update()
    {
        bool isTriggerPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller) > 0.1f;

        // ONE-SHOT logic
        if (isColliding && isTriggerPressed && !wasTriggerPressed)
        {
            taskInteraction.OnSuccess();
            taskInteraction.SetMeshMaterial(greenMaterial);
        }
        else if (!isTriggerPressed || !isColliding)
        {
            taskInteraction.SetMeshMaterial(transparentMaterial);
        }

        wasTriggerPressed = isTriggerPressed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            isColliding = false;
        }
    }
}