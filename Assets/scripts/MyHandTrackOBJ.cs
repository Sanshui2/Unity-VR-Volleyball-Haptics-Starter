using Unity.XR.PXR;
using UnityEngine;

public class MyHandTrackOBJ : MonoBehaviour
{
    public PXR_Hand xR_Hand;
    public HandJoint trackTyoe;

    private Transform traget;

    private void Start()
    {
        if (xR_Hand == null)
        {
            Debug.LogError("MyHandTrackOBJ requires a PXR_Hand reference.", this);
            enabled = false;
            return;
        }

        traget = xR_Hand.handJoints[(int)trackTyoe];
    }

    private void Update()
    {
        if (traget == null)
        {
            return;
        }

        transform.SetPositionAndRotation(traget.position, traget.rotation);
    }
}
