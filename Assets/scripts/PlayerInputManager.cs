using System.Collections;
using Unity.XR.PXR;
using UnityEngine;

/// <summary>
/// Connects two Pico Motion Trackers and assigns them to the left and right hand proxies.
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    public enum HandType { None, Left, Right }

    [Header("Tracker Proxy References")]
    [Tooltip("Proxy object used as the left-hand hit target after calibration.")]
    public TrackerProxy leftTrackerProxy;
    [Tooltip("Proxy object used as the right-hand hit target after calibration.")]
    public TrackerProxy rightTrackerProxy;

    [Header("Calibration")]
    [Tooltip("Head transform used to determine which tracker is left or right.")]
    public Transform headTransform;
    [Tooltip("Minimum horizontal separation required before left/right can be assigned.")]
    public float calibrationMinSeparation = 0.2f;
    [Tooltip("Request two Pico Motion Trackers automatically on scene load.")]
    public bool autoConnectOnStart = true;

    private Coroutine calibrationCoroutine;
    private bool callbacksRegistered;

    private void OnEnable()
    {
        RegisterCallbacks();
    }

    private void Start()
    {
        if (headTransform == null && Camera.main != null)
        {
            headTransform = Camera.main.transform;
        }

        if (autoConnectOnStart)
        {
            ConnectTrackers();
        }
    }

    private void OnDisable()
    {
        if (calibrationCoroutine != null)
        {
            StopCoroutine(calibrationCoroutine);
            calibrationCoroutine = null;
        }

        UnregisterCallbacks();
    }

    public void ConnectTrackers()
    {
        if (leftTrackerProxy == null || rightTrackerProxy == null)
        {
            Debug.LogError("Assign both tracker proxies before requesting Motion Trackers.", this);
            return;
        }

        if (headTransform == null)
        {
            Debug.LogError("Assign a head transform before calibrating Motion Trackers.", this);
            return;
        }

        PXR_MotionTracking.CheckMotionTrackerNumber(MotionTrackerNum.TWO);
        Debug.Log("Requesting two Pico Motion Trackers...");
    }

    private void RegisterCallbacks()
    {
        if (callbacksRegistered)
        {
            return;
        }

        PXR_MotionTracking.RequestMotionTrackerCompleteAction += OnRequestMotionTrackerComplete;
        callbacksRegistered = true;
    }

    private void UnregisterCallbacks()
    {
        if (!callbacksRegistered)
        {
            return;
        }

        PXR_MotionTracking.RequestMotionTrackerCompleteAction -= OnRequestMotionTrackerComplete;
        callbacksRegistered = false;
    }

    private void OnRequestMotionTrackerComplete(RequestMotionTrackerCompleteEventData eventData)
    {
        if (eventData.trackerCount < 2)
        {
            Debug.LogError("Two Motion Trackers are required for tracker-enhanced mode.", this);
            return;
        }

        if (calibrationCoroutine != null)
        {
            StopCoroutine(calibrationCoroutine);
        }

        calibrationCoroutine = StartCoroutine(
            CalibrateHandsCoroutine(eventData.trackerIds[0], eventData.trackerIds[1]));
    }

    private IEnumerator CalibrateHandsCoroutine(long firstTrackerId, long secondTrackerId)
    {
        while (enabled)
        {
            MotionTrackerLocation firstLocation = new MotionTrackerLocation();
            MotionTrackerLocation secondLocation = new MotionTrackerLocation();
            bool firstValid = false;
            bool secondValid = false;

            PXR_MotionTracking.GetMotionTrackerLocation(firstTrackerId, ref firstLocation, ref firstValid);
            PXR_MotionTracking.GetMotionTrackerLocation(secondTrackerId, ref secondLocation, ref secondValid);

            if (firstValid && secondValid)
            {
                Vector3 firstWorldPosition = transform.TransformPoint(firstLocation.pose.Position.ToVector3());
                Vector3 secondWorldPosition = transform.TransformPoint(secondLocation.pose.Position.ToVector3());

                Vector3 firstRelativeToHead = headTransform.InverseTransformPoint(firstWorldPosition);
                Vector3 secondRelativeToHead = headTransform.InverseTransformPoint(secondWorldPosition);

                if (Mathf.Abs(firstRelativeToHead.x - secondRelativeToHead.x) > calibrationMinSeparation)
                {
                    bool firstIsLeft = firstRelativeToHead.x < secondRelativeToHead.x;
                    AssignTrackers(
                        firstIsLeft ? firstTrackerId : secondTrackerId,
                        firstIsLeft ? secondTrackerId : firstTrackerId);

                    calibrationCoroutine = null;
                    yield break;
                }
            }

            yield return null;
        }
    }

    private void AssignTrackers(long leftTrackerId, long rightTrackerId)
    {
        leftTrackerProxy.preferredHandType = HandType.Left;
        rightTrackerProxy.preferredHandType = HandType.Right;

        leftTrackerProxy.Initialize(leftTrackerId, transform);
        rightTrackerProxy.Initialize(rightTrackerId, transform);

        Debug.Log("<color=green>Motion Tracker calibration complete. Left and right hands assigned.</color>", this);
    }
}
