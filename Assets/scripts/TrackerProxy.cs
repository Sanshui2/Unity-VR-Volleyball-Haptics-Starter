using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

/// <summary>
/// Hand hit proxy that can either follow an external transform or drive itself from a Pico Motion Tracker.
/// </summary>
public class TrackerProxy : MonoBehaviour
{
    public enum TrackingSource
    {
        ExternalTransform,
        PicoMotionTracker
    }

    [Header("Hand Identity")]
    [Tooltip("Prefer explicit assignment. When left as None, the component infers side from tag or name.")]
    public PlayerInputManager.HandType preferredHandType = PlayerInputManager.HandType.None;

    [Header("Tracking")]
    [Tooltip("How this proxy receives motion updates.")]
    public TrackingSource trackingSource = TrackingSource.ExternalTransform;
    [Tooltip("How many historical poses are used to smooth velocity.")]
    [Range(2, 12)]
    public int velocityBufferSize = 5;

    private readonly Queue<PoseData> poseHistory = new Queue<PoseData>();
    private bool isInitialized;
    private bool hasValidPose = true;
    private long trackerId;
    private Transform xrOriginTransform;
    private Vector3 worldVelocity;

    private struct PoseData
    {
        public Vector3 Position;
        public float Timestamp;
    }

    private void OnEnable()
    {
        ResetVelocityHistory();
    }

    private void Update()
    {
        if (trackingSource == TrackingSource.PicoMotionTracker && isInitialized)
        {
            UpdateFromMotionTracker();
        }

        UpdateVelocity();
    }

    public PlayerInputManager.HandType GetHandType()
    {
        if (preferredHandType != PlayerInputManager.HandType.None)
        {
            return preferredHandType;
        }

        if (CompareTag("LeftHand") || gameObject.name.Contains("Left"))
        {
            return PlayerInputManager.HandType.Left;
        }

        if (CompareTag("RightHand") || gameObject.name.Contains("Right"))
        {
            return PlayerInputManager.HandType.Right;
        }

        return PlayerInputManager.HandType.None;
    }

    public Vector3 GetVelocity()
    {
        return worldVelocity;
    }

    public void Initialize(long newTrackerId, Transform originTransform)
    {
        trackerId = newTrackerId;
        xrOriginTransform = originTransform;
        trackingSource = TrackingSource.PicoMotionTracker;
        isInitialized = true;
        hasValidPose = false;
        ResetVelocityHistory();
    }

    public void UseExternalTransform()
    {
        trackingSource = TrackingSource.ExternalTransform;
        isInitialized = false;
        hasValidPose = true;
        ResetVelocityHistory();
    }

    private void UpdateFromMotionTracker()
    {
        if (xrOriginTransform == null)
        {
            hasValidPose = false;
            return;
        }

        MotionTrackerLocation location = new MotionTrackerLocation();
        bool isValidPose = false;
        int result = PXR_MotionTracking.GetMotionTrackerLocation(trackerId, ref location, ref isValidPose);

        if (result != 0 || !isValidPose)
        {
            hasValidPose = false;
            return;
        }

        Vector3 localPosition = location.pose.Position.ToVector3();
        Quaternion localRotation = location.pose.Orientation.ToQuat();
        transform.SetPositionAndRotation(
            xrOriginTransform.TransformPoint(localPosition),
            xrOriginTransform.rotation * localRotation);

        hasValidPose = true;
    }

    private void UpdateVelocity()
    {
        if (trackingSource == TrackingSource.PicoMotionTracker && isInitialized && !hasValidPose)
        {
            worldVelocity = Vector3.zero;
            return;
        }

        poseHistory.Enqueue(new PoseData
        {
            Position = transform.position,
            Timestamp = Time.time
        });

        while (poseHistory.Count > Mathf.Max(2, velocityBufferSize))
        {
            poseHistory.Dequeue();
        }

        if (poseHistory.Count < 2)
        {
            worldVelocity = Vector3.zero;
            return;
        }

        PoseData oldest = poseHistory.Peek();
        float timeSpan = Time.time - oldest.Timestamp;

        if (timeSpan <= float.Epsilon)
        {
            worldVelocity = Vector3.zero;
            return;
        }

        worldVelocity = (transform.position - oldest.Position) / timeSpan;
    }

    private void ResetVelocityHistory()
    {
        poseHistory.Clear();
        worldVelocity = Vector3.zero;
        poseHistory.Enqueue(new PoseData
        {
            Position = transform.position,
            Timestamp = Time.time
        });
    }
}
