using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hand hit proxy used by the starter scene.
/// It does not own tracking input anymore; it only samples transform motion
/// so the same component can work with hand tracking, controllers, or custom rigs.
/// </summary>
public class TrackerProxy : MonoBehaviour
{
    [Header("Hand Identity")]
    [Tooltip("Prefer explicit assignment. When left as None, the component infers side from tag/name.")]
    public PlayerInputManager.HandType preferredHandType = PlayerInputManager.HandType.None;

    [Header("Velocity Sampling")]
    [Tooltip("How many historical poses are used to smooth velocity.")]
    [Range(2, 12)]
    public int velocityBufferSize = 5;

    private readonly Queue<PoseData> poseHistory = new Queue<PoseData>();
    private Vector3 worldVelocity;

    private struct PoseData
    {
        public Vector3 Position;
        public float Timestamp;
    }

    private void OnEnable()
    {
        poseHistory.Clear();
        worldVelocity = Vector3.zero;
        poseHistory.Enqueue(new PoseData
        {
            Position = transform.position,
            Timestamp = Time.time
        });
    }

    private void Update()
    {
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
        // Kept only for compatibility with the disabled legacy tracker manager.
    }

    private void UpdateVelocity()
    {
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
}
