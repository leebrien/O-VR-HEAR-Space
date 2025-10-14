using UnityEngine;

public class SaberBlade : MonoBehaviour
{
    public string targetTag = "CueSource";
    public Material greenMaterial;
    public Material transparentMaterial;
    public OVRInput.Controller controller; // Left or Right controller
    public TaskInteraction taskInteraction; // Must have OnSuccess() and SetMeshMaterial(Material)
    public LayerMask targetLayer;

    private bool _isColliding = false;

    void Start()
    {
        Debug.Log("[SaberBlade] Script initialized on " + gameObject.name + " with controller: " + controller);
    }

    void Update()
    {
        bool isTriggerPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller) > 0.1f;

        // Debug trigger input state
        if (isTriggerPressed)
            Debug.Log("[SaberBlade] Trigger pressed on " + controller);
        

        // ONE-SHOT logic
        if (_isColliding && isTriggerPressed )
        {
            Debug.Log("[SaberBlade] SUCCESS: Trigger pulled while colliding with target!");
            if (taskInteraction)
            {
                taskInteraction.OnSuccess();
                taskInteraction.SetMeshMaterial(greenMaterial);
            }
            else
            {
                Debug.LogWarning("[SaberBlade] taskInteraction not assigned!");
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0 && other.CompareTag(targetTag))
        {
            _isColliding = true;
            Debug.Log("[SaberBlade] OnTriggerEnter with " + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0 && other.CompareTag(targetTag))
        {
            _isColliding = false;
            Debug.Log("[SaberBlade] OnTriggerExit with " + other.name);
        }
    }
}
