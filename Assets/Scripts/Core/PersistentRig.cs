using UnityEngine;

public class PersistentRig : MonoBehaviour
{
    private static PersistentRig _instance;
    public GameObject ovrHands;
    public GameObject ovrControllers;
    public GameObject leftXRRayInteractor;
    public GameObject rightXRRayInteractor;
    public GameObject leftSaber;
    public GameObject rightSaber;
    public Transform centerEyeAnchor;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void Start()
    {
        if (CoreManager.Instance != null)
        {
            CoreManager.Instance.SetRigReferences(gameObject, ovrHands, ovrControllers, leftXRRayInteractor,
                rightXRRayInteractor, leftSaber, rightSaber, centerEyeAnchor);
        }
    }
}