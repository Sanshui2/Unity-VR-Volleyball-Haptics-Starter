using UnityEngine;

/// <summary>
/// Minimal hand reference container for the starter branch.
/// Tracker-specific calibration lives outside main so this project can stay focused on hand volleyball.
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    public enum HandType { None, Left, Right }

    [Header("Hand Proxy References")]
    public TrackerProxy leftTrackerProxy;
    public TrackerProxy rightTrackerProxy;

    [Header("Optional Head Reference")]
    public Transform headTransform;
}
