using UnityEngine;

public class SaberManager : MonoBehaviour
{
    public SaberBlade leftSaber;
    public SaberBlade rightSaber;

    public void EnableSabers()
    {
        leftSaber.enabled = true;
        rightSaber.enabled = true;
    }

    public void DisableSabers()
    {
        leftSaber.enabled = false;
        rightSaber.enabled = false;
    }

}
