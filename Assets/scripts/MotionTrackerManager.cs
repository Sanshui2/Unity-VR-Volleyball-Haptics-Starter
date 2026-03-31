using Unity.XR.PXR;
using UnityEngine;

/// <summary>
/// Optional one-tracker debug helper kept on the tracker-enhanced branch.
/// </summary>
public class MotionTrackerManager : MonoBehaviour
{
    [Tooltip("Assign the XR Origin transform used to convert tracker local space into world space.")]
    public Transform xrOriginTransform;

    [Header("Debug Toggle")]
    [Tooltip("Enable after a tracker has connected to stream its pose into the visual cube.")]
    public bool attemptToGetLocation = false;

    [Header("Debug Visual")]
    [Tooltip("Optional object used to preview the connected tracker pose.")]
    public Transform visualCube;

    private bool isConnected;
    private long trackerId;

    private void Start()
    {
        if (visualCube != null)
        {
            visualCube.gameObject.SetActive(false);
        }

        PXR_MotionTracking.RequestMotionTrackerCompleteAction += OnRequestMotionTrackerComplete;
        PXR_MotionTracking.CheckMotionTrackerNumber(MotionTrackerNum.ONE);
        Debug.Log("Requesting one Motion Tracker for debug preview...");
    }

    private void Update()
    {
        if (!isConnected || !attemptToGetLocation || xrOriginTransform == null || visualCube == null)
        {
            return;
        }

        MotionTrackerLocation location = new MotionTrackerLocation();
        bool isValidPose = false;
        int result = PXR_MotionTracking.GetMotionTrackerLocation(trackerId, ref location, ref isValidPose);

        if (result != 0 || !isValidPose)
        {
            if (Time.frameCount % 60 == 0)
            {
                Debug.LogWarning($"Motion Tracker pose unavailable. Result={result}, Valid={isValidPose}", this);
            }

            return;
        }

        Vector3 localPosition = location.pose.Position.ToVector3();
        Quaternion localRotation = location.pose.Orientation.ToQuat();
        visualCube.gameObject.SetActive(true);
        visualCube.SetPositionAndRotation(
            xrOriginTransform.TransformPoint(localPosition),
            xrOriginTransform.rotation * localRotation);
    }

    private void OnDestroy()
    {
        PXR_MotionTracking.RequestMotionTrackerCompleteAction -= OnRequestMotionTrackerComplete;
    }

    private void OnRequestMotionTrackerComplete(RequestMotionTrackerCompleteEventData eventData)
    {
        if (eventData.trackerCount <= 0)
        {
            Debug.LogError("No Motion Tracker found for debug preview.", this);
            return;
        }

        isConnected = true;
        trackerId = eventData.trackerIds[0];
        Debug.Log($"Motion Tracker debug preview connected. Tracker ID: {trackerId}", this);
    }
}
