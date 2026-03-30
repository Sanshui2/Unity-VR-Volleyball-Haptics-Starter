using System;
using UnityEngine;

/// <summary>
/// Minimal volleyball controller for the starter scene.
/// Supports bump and underhand serve using hand proxy velocity.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class VolleyballController : MonoBehaviour
{
    public enum HitType { Bump, Spike, UnderhandServe }

    public static event Action<HitType, PlayerInputManager.HandType> OnBallHit;

    [Header("Core References")]
    [Tooltip("Legacy experiment reference. Starter mode can leave this empty.")]
    public PlayerInputManager playerInputManager;
    [Tooltip("Optional: assign left hand proxy directly. Auto-resolved when empty.")]
    public TrackerProxy leftHandProxy;
    [Tooltip("Optional: assign right hand proxy directly. Auto-resolved when empty.")]
    public TrackerProxy rightHandProxy;
    [Tooltip("Head transform used to evaluate hit height.")]
    public Transform headTransform;
    [Tooltip("Approximate opponent court target for spike assistance.")]
    public Transform opponentCourtTarget;

    [Header("Modes")]
    [Tooltip("Whether to enable the legacy overhand spike check.")]
    public bool enableOverhandSpike = false;
    [Tooltip("Whether to enable assisted spike velocity.")]
    public bool perfectShotModeEnabled = true;
    [Tooltip("Maximum hand separation for bump detection.")]
    public float bumpMaxDistance = 0.5f;
    [Tooltip("Minimum downward speed for spike detection.")]
    public float spikeDownwardVelocity = 1.0f;
    [Tooltip("Minimum overall swing speed for spike detection.")]
    public float spikeVelocityThreshold = 3.5f;

    [Header("Hit Force")]
    [Tooltip("Force multiplier for bump hits.")]
    public float bumpForceMultiplier = 1.2f;
    [Tooltip("Assisted spike launch speed.")]
    public float perfectShotSpeed = 15.0f;
    [Tooltip("Assisted spike vertical angle.")]
    public float perfectShotAngle = 20.0f;
    [Tooltip("Force multiplier for underhand serve hits.")]
    public float underhandServeForceMultiplier = 1.3f;

    [Header("Underhand Serve Detection")]
    [Tooltip("Minimum upward speed for underhand serve detection.")]
    public float underhandServeMinUpwardVelocity = 0.5f;
    [Tooltip("Minimum swing speed for underhand serve detection.")]
    public float underhandServeVelocityThreshold = 2.0f;
    [Tooltip("Hit height must stay below this value relative to the head.")]
    public float underhandServeMaxHeightRelative = -0.2f;

    [Header("bHaptics (Optional)")]
    [Tooltip("Enable bHaptics playback when the ball is hit.")]
    public bool enableBhaptics = false;
    [Tooltip("Event ID played for left-hand contact.")]
    public string leftHandEventId;
    [Tooltip("Event ID played for right-hand contact.")]
    public string rightHandEventId;

    private Rigidbody rb;
    private float lastHitTime = -1f;
    private const float HitCooldown = 0.2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (headTransform == null && Camera.main != null)
        {
            headTransform = Camera.main.transform;
        }

        TryResolveHandReferences();

        if (headTransform == null || opponentCourtTarget == null)
        {
            Debug.LogError("Please assign headTransform and opponentCourtTarget.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < lastHitTime + HitCooldown)
        {
            return;
        }

        TrackerProxy hittingHandProxy = other.GetComponent<TrackerProxy>();
        if (hittingHandProxy == null)
        {
            return;
        }

        TryResolveHandReferences();

        TrackerProxy resolvedLeftHand = ResolveHandProxy(PlayerInputManager.HandType.Left);
        TrackerProxy resolvedRightHand = ResolveHandProxy(PlayerInputManager.HandType.Right);
        if (resolvedLeftHand == null || resolvedRightHand == null || headTransform == null)
        {
            Debug.LogWarning("Missing hand proxies or head transform. Ignoring hit.", this);
            return;
        }

        lastHitTime = Time.time;

        Vector3 leftHandPos = resolvedLeftHand.transform.position;
        Vector3 rightHandPos = resolvedRightHand.transform.position;
        float distanceBetweenHands = Vector3.Distance(leftHandPos, rightHandPos);
        Vector3 hittingHandVelocity = hittingHandProxy.GetVelocity();
        PlayerInputManager.HandType hand = hittingHandProxy.GetHandType();
        float handHeightRelativeToHead = headTransform.InverseTransformPoint(hittingHandProxy.transform.position).y;

        HitType hitType;
        if (enableOverhandSpike &&
            distanceBetweenHands >= bumpMaxDistance &&
            hittingHandVelocity.y < -spikeDownwardVelocity &&
            hittingHandVelocity.magnitude > spikeVelocityThreshold)
        {
            hitType = HitType.Spike;
            HandleSpike(hittingHandProxy);
        }
        else if (distanceBetweenHands >= bumpMaxDistance &&
                 hittingHandVelocity.y > underhandServeMinUpwardVelocity &&
                 hittingHandVelocity.magnitude > underhandServeVelocityThreshold &&
                 handHeightRelativeToHead < underhandServeMaxHeightRelative)
        {
            hitType = HitType.UnderhandServe;
            HandleUnderhandServe(hittingHandProxy);
        }
        else
        {
            hitType = HitType.Bump;
            HandleBump();
        }

        PlayBhaptics(hand);
        OnBallHit?.Invoke(hitType, hand);
    }

    private void HandleSpike(TrackerProxy spikeHand)
    {
        if (perfectShotModeEnabled && opponentCourtTarget != null)
        {
            Vector3 direction = (opponentCourtTarget.position - transform.position).normalized;
            direction = Quaternion.AngleAxis(-perfectShotAngle, transform.right) * direction;
            rb.velocity = direction * perfectShotSpeed;
            return;
        }

        rb.velocity = spikeHand.GetVelocity() * 1.5f;
    }

    private void HandleUnderhandServe(TrackerProxy serveHand)
    {
        rb.velocity = serveHand.GetVelocity() * underhandServeForceMultiplier;
    }

    private void HandleBump()
    {
        TrackerProxy resolvedLeftHand = ResolveHandProxy(PlayerInputManager.HandType.Left);
        TrackerProxy resolvedRightHand = ResolveHandProxy(PlayerInputManager.HandType.Right);
        if (resolvedLeftHand == null || resolvedRightHand == null)
        {
            return;
        }

        Vector3 leftVel = resolvedLeftHand.GetVelocity();
        Vector3 rightVel = resolvedRightHand.GetVelocity();
        Vector3 bumpVelocity = (leftVel + rightVel) / 2f;

        rb.velocity = bumpVelocity * bumpForceMultiplier;
    }

    private void PlayBhaptics(PlayerInputManager.HandType handType)
    {
        if (!enableBhaptics)
        {
            return;
        }

        string eventId = handType == PlayerInputManager.HandType.Left ? leftHandEventId : rightHandEventId;
        if (string.IsNullOrWhiteSpace(eventId))
        {
            return;
        }

        BhapticsBridge.TryPlayEvent(eventId);
    }

    private TrackerProxy ResolveHandProxy(PlayerInputManager.HandType handType)
    {
        if (handType == PlayerInputManager.HandType.Left && leftHandProxy != null && leftHandProxy.isActiveAndEnabled)
        {
            return leftHandProxy;
        }

        if (handType == PlayerInputManager.HandType.Right && rightHandProxy != null && rightHandProxy.isActiveAndEnabled)
        {
            return rightHandProxy;
        }

        return null;
    }

    private void TryResolveHandReferences()
    {
        if (leftHandProxy != null && leftHandProxy.isActiveAndEnabled &&
            rightHandProxy != null && rightHandProxy.isActiveAndEnabled)
        {
            return;
        }

        if (playerInputManager != null)
        {
            if (leftHandProxy == null &&
                playerInputManager.leftTrackerProxy != null &&
                playerInputManager.leftTrackerProxy.isActiveAndEnabled)
            {
                leftHandProxy = playerInputManager.leftTrackerProxy;
            }

            if (rightHandProxy == null &&
                playerInputManager.rightTrackerProxy != null &&
                playerInputManager.rightTrackerProxy.isActiveAndEnabled)
            {
                rightHandProxy = playerInputManager.rightTrackerProxy;
            }
        }

        TrackerProxy[] proxies = FindObjectsOfType<TrackerProxy>();
        foreach (TrackerProxy proxy in proxies)
        {
            if (!proxy.isActiveAndEnabled)
            {
                continue;
            }

            PlayerInputManager.HandType handType = proxy.GetHandType();
            if (leftHandProxy == null && handType == PlayerInputManager.HandType.Left)
            {
                leftHandProxy = proxy;
            }
            else if (rightHandProxy == null && handType == PlayerInputManager.HandType.Right)
            {
                rightHandProxy = proxy;
            }
        }
    }
}
