using UnityEngine;
using UnityEngine.SceneManagement;

public class SaberBlade : MonoBehaviour
{
    public string targetTag = "CueSource";
    public Material greenMaterial;
    public OVRInput.Controller controller; // Left or Right controller
    private TaskInteraction _taskInteraction; // Must have OnSuccess() and SetMeshMaterial(Material)
    private PracticeSoundManager _practiceSoundManager;
    public LayerMask targetLayer;

    private bool _isColliding = false;
    
    private void OnEnable()
        {
            // Reset the collision state to ensure we start fresh in each new scene.
            _isColliding = false;
            //Debug.Log("[SaberBlade] OnEnable: Collision state has been reset.");
        }
    

    public void UpdateSceneReferences(PracticeSoundManager practiceSoundManager, TaskInteraction taskInteraction)
    {
        //Debug.Log("UpdateSceneReferences on " + SceneManager.GetActiveScene().name + " with practiceSoundManager: " 
                  //+ practiceSoundManager + " and taskInteraction: " + taskInteraction);
        _practiceSoundManager = practiceSoundManager;
        _taskInteraction = taskInteraction;
        
    }

    private void Update()
    {
        bool isTriggerPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller) > 0.1f;
        
        //if (isTriggerPressed)
        //{
         //   Debug.Log("SaberBlade triggered");
        //}

        // ONE-SHOT logic
        if (_isColliding && isTriggerPressed)
        {
            //Debug.Log("Successful Trigger");
            //Debug.Log("TaskInteraction is " + (_taskInteraction != null ? "notnull" : "null"));
            //Debug.Log("PracticeSoundManager is " + (_practiceSoundManager != null ? "notnull" : "null"));
            if (_taskInteraction != null)
            {
                _taskInteraction.OnSuccess();
            } else if (_practiceSoundManager != null)
            {
                _practiceSoundManager.OnSuccess(greenMaterial);
            }
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0 && other.CompareTag(targetTag))
        {
            _isColliding = true;
            //Debug.Log("[SaberBlade] OnTriggerEnter with " + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0 && other.CompareTag(targetTag))
        {
            _isColliding = false;
            //Debug.Log("[SaberBlade] OnTriggerExit with " + other.name);
        }
    }
}
