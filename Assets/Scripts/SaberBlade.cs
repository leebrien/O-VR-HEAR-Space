using UnityEngine;

public class SaberBlade : MonoBehaviour
{
    public string targetTag = "CueSource";
    public Material greenMaterial;
    public OVRInput.Controller controller; // Left or Right controller
    public TaskInteraction taskInteraction; // Must have OnSuccess() and SetMeshMaterial(Material)
    public PracticeSoundManager practiceSoundManager;
    public LayerMask targetLayer;

    private bool _isColliding = false;
    private string _sceneName;

    void Start()
    {
        _sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        bool isTriggerPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller) > 0.1f;

        // ONE-SHOT logic
        if (_isColliding && isTriggerPressed)
        {
            if (taskInteraction)
            {
                taskInteraction.OnSuccess();
                taskInteraction.SetMeshMaterial(greenMaterial);
            } else if (_sceneName.Contains("Practice"))
            {
                practiceSoundManager.OnSuccess(greenMaterial);
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
