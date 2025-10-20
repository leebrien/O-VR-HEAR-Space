using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackingSwitcher : MonoBehaviour
{
    private GameObject _ovrHands;
    private GameObject _ovrControllers;

    private void Awake()
    {
        _ovrHands = CoreManager.Instance.GetHands();
        _ovrControllers = CoreManager.Instance.GetControllers();
        
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Contains("Questionnaire"))
        {
            SwitchToBoth();
        }
    }

    public void SwitchToHandsOnly()
    {
        _ovrHands.SetActive(true);
        _ovrControllers.SetActive(false);
    }

    public void SwitchToControllersOnly()
    {
        _ovrHands.SetActive(false);
        _ovrControllers.SetActive(true);
    }

    public void SwitchToBoth()
    {
        _ovrHands.SetActive(true);
        _ovrControllers.SetActive(true);
    }
}
