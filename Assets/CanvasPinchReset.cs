using UnityEngine;
using Oculus.Interaction;

public class CanvasPinchReset : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRHand rightHand;

    public Transform xrCamera;
    public float recenterDistance = 2.0f;

    void Update()
    {
        if (leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index) || rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            RecenterCanvas();
        }
    }

    private void RecenterCanvas()
    {
        Vector3 cameraPosition = xrCamera.position;
        Vector3 cameraForward = xrCamera.forward;

        Vector3 newPosition = cameraPosition + cameraForward * recenterDistance;

        transform.position = newPosition;
        transform.rotation = Quaternion.LookRotation(newPosition - cameraPosition);
    }
}