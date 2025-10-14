using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SaberManager : MonoBehaviour
{
    public GameObject leftSaber;
    public GameObject rightSaber;
    public GameObject leftXRRayInteractor = null;
    public GameObject rightXRRayInteractor = null;
    
    
    public void EnableSabers()
    {
        leftSaber.SetActive(true);
        rightSaber.SetActive(true);
        leftXRRayInteractor.SetActive(false);
        rightXRRayInteractor.SetActive(false);
        
    }

    public void DisableSabers()
    {
        leftSaber.SetActive(false);
        rightSaber.SetActive(false);
        leftXRRayInteractor.SetActive(true);
        rightXRRayInteractor.SetActive(true);
    }

}