using UnityEngine;

public class TrackingSwitcher : MonoBehaviour
{
    public GameObject ovrHands;
    public GameObject ovrControllers;

    public void SwitchToHandsOnly()
    {
        ovrHands.SetActive(true);
        ovrControllers.SetActive(false);
    }

    public void SwitchToControllersOnly()
    {
        ovrHands.SetActive(false);
        ovrControllers.SetActive(true);
    }

    public void SwitchToBoth()
    {
        ovrHands.SetActive(true);
        ovrControllers.SetActive(true);
    }
}
