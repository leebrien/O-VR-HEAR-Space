using UnityEngine;

public class SaberBlade : MonoBehaviour
{
    public string targetTag = "CueSource";
    public Material greenMaterial;
    public Material transparentMaterial;
    public OVRInput.Controller controller; // Left or Right controller
    public TaskInteraction taskInteraction; // Must have OnSuccess() and SetMeshMaterial(Material)

    private bool isColliding = false;
    private bool wasTriggerPressed = false;

    void Start()
    {
        Debug.Log("[SaberBlade] Script initialized on " + gameObject.name + " with controller: " + controller);
    }

    void Update()
    {
        bool isTriggerPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller) > 0.1f;

        // Debug trigger input state
        if (isTriggerPressed && !wasTriggerPressed)
            Debug.Log("[SaberBlade] Trigger pressed on " + controller);

        if (!isTriggerPressed && wasTriggerPressed)
            Debug.Log("[SaberBlade] Trigger released on " + controller);

        // ONE-SHOT logic
        if (isColliding && isTriggerPressed && !wasTriggerPressed)
        {
            Debug.Log("[SaberBlade] SUCCESS: Trigger pulled while colliding with target!");
            if (taskInteraction != null)
            {
                taskInteraction.OnSuccess();
                taskInteraction.SetMeshMaterial(greenMaterial);
            }
            else
            {
                Debug.LogWarning("[SaberBlade] taskInteraction not assigned!");
            }
        }
        else if (!isTriggerPressed || !isColliding)
        {
            if (taskInteraction != null)
                taskInteraction.SetMeshMaterial(transparentMaterial);
        }

        wasTriggerPressed = isTriggerPressed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer != 0 && other.CompareTag(targetTag))
        {
            isColliding = true;
            Debug.Log("[SaberBlade] OnTriggerEnter with " + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (1 << other.gameObject.layer != 0 && other.CompareTag(targetTag))
        {
            isColliding = false;
            Debug.Log("[SaberBlade] OnTriggerExit with " + other.name);
        }
    }
}
