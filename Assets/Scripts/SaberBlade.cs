using UnityEngine;

public class SaberBlade : MonoBehaviour
{
    public string targetTag = "CueSource";
    public Material greenMaterial;
    public Material transparentMaterial;
    public float saberLength = 4.5f;
    public OVRInput.Controller controller;
    public TaskInteraction taskInteraction;

    private Collider _currentTarget;

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool hasHit = Physics.Raycast(ray, out hit, saberLength);

        if (hasHit && hit.collider.CompareTag(targetTag))
        {
            _currentTarget = hit.collider;

            bool triggerHeld = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller);

            
            if (triggerHeld)
            {
                taskInteraction.SetMeshMaterial(greenMaterial);
            }
            else
            {
                taskInteraction.SetMeshMaterial(transparentMaterial);
            }

            // Trigger success
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
            {
                Debug.Log($"{controller}: Saber ray hit target {hit.collider.name}");
                taskInteraction.OnSuccess();
            }
        }
        else
        {
            _currentTarget = null;
        }
        
    }
}
