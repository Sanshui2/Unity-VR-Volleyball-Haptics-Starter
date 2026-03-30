using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Optional bHaptics bridge that avoids a hard compile-time dependency on the SDK.
/// Import the official bHaptics Unity SDK if you want playback to work.
/// </summary>
public static class BhapticsBridge
{
    private static readonly Type LibraryType = Type.GetType("Bhaptics.SDK2.BhapticsLibrary, Bhaptics.SDK2");
    private static readonly MethodInfo PlayMethod = LibraryType?.GetMethod(
        "Play",
        BindingFlags.Public | BindingFlags.Static,
        null,
        new[] { typeof(string) },
        null);

    private static bool missingSdkWarningShown;

    public static bool TryPlayEvent(string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
        {
            return false;
        }

        if (PlayMethod == null)
        {
            if (!missingSdkWarningShown)
            {
                Debug.LogWarning("bHaptics playback requested, but the bHaptics Unity SDK is not installed.");
                missingSdkWarningShown = true;
            }

            return false;
        }

        try
        {
            PlayMethod.Invoke(null, new object[] { eventId });
            return true;
        }
        catch (TargetInvocationException ex)
        {
            Debug.LogWarning($"bHaptics playback failed for event '{eventId}': {ex.InnerException?.Message ?? ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"bHaptics playback failed for event '{eventId}': {ex.Message}");
            return false;
        }
    }
}
