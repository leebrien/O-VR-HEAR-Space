// csharp
using UnityEngine;

public class SaberManager : MonoBehaviour
{
    private GameObject _leftSaber;
    private GameObject _rightSaber;
    private GameObject _leftXRRayInteractor;
    private GameObject _rightXRRayInteractor;

    private void Awake()
    {
        RefreshRefs(); // earliest chance to resolve references
    }

    private void OnEnable()
    {
        // re-try when enabled (after scene loads or toggles)
        if (AnyNull()) RefreshRefs();
    }

    /*private bool AnyNull()
    {
        return _leftSaber is null || _rightSaber is null ||
               _leftXRRayInteractor is null || _rightXRRayInteractor is null;
    }*/ 
    private bool AnyNull()
    {
        // Use '==' for UnityEngine.Object types
        return _leftSaber == null || _rightSaber == null ||
               _leftXRRayInteractor == null || _rightXRRayInteractor == null;
    }

    private void RefreshRefs()
    {
        if (!CoreManager.Instance) return;

        _leftSaber = CoreManager.Instance.GetLeftSaber();
        _rightSaber = CoreManager.Instance.GetRightSaber();
        _leftXRRayInteractor = CoreManager.Instance.GetLeftXRRayInteractor();
        _rightXRRayInteractor = CoreManager.Instance.GetRightXRRayInteractor();
    }

    public void EnableSabers()
    {
        if (AnyNull()) RefreshRefs();
        if (_leftSaber == null || _rightSaber == null) return;

        _leftSaber.SetActive(true);
        _rightSaber.SetActive(true);

        //if (AnyNull()) RefreshRefs();
        if (_leftXRRayInteractor != null) _leftXRRayInteractor.SetActive(false);
        if (_rightXRRayInteractor != null) _rightXRRayInteractor.SetActive(false);
    }

    /*public void DisableSabers()
    {
        if (AnyNull()) RefreshRefs();

        _leftSaber?.SetActive(false);
        _rightSaber?.SetActive(false);

        if (AnyNull()) RefreshRefs();
        _leftXRRayInteractor?.SetActive(true);
        _rightXRRayInteractor?.SetActive(true);
    }*/
    public void DisableSabers()
    {
        if (AnyNull()) RefreshRefs();
        
        if (_leftSaber != null)
        {
            _leftSaber.SetActive(false);
        }
        if (_rightSaber != null)
        {
            _rightSaber.SetActive(false);
        }
        

        if (_leftXRRayInteractor != null)
        {
            _leftXRRayInteractor.SetActive(true);
        }
        if (_rightXRRayInteractor != null)
        {
            _rightXRRayInteractor.SetActive(true);
        }
    }
}