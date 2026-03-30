# bHaptics Setup on `main`

This repository keeps `main` as a clean starter branch.
That means the project exposes bHaptics hook points, but does not bundle the bHaptics Unity SDK itself.

If you want haptic playback on `main`, follow this setup flow.

## What Is Already In The Starter

- `Assets/scripts/VolleyballController.cs` exposes:
  - `Enable Bhaptics`
  - `Left Hand Event Id`
  - `Right Hand Event Id`
- `Assets/scripts/BhapticsBridge.cs` lets the project compile even when the bHaptics SDK is not installed.

If the SDK is missing, the project will skip playback instead of failing to compile.

## Setup Steps

1. Install the official bHaptics Unity plugin from the Unity Asset Store.
2. Open the Unity toolbar and go to `bHaptics -> Developer Window`.
3. Link your haptic app with the `App ID` and `API Key` from the bHaptics Developer Portal.
4. Make sure your haptic app has deployed events. The starter expects you to reference deployed event IDs.
5. Add the `[bhaptics]` prefab into the first scene that boots your project.
   - Official SDK path: `Assets/Bhaptics/SDK2/Prefabs/[bhaptics].prefab`
6. On Android projects with target API 30+, update `Assets/Plugins/Android/AndroidManifest.xml`:
   - add the `com.bhaptics.player` query
   - add Bluetooth permissions required by the SDK
7. Open the volleyball prefab or scene instance and configure:
   - `Enable Bhaptics`
   - `Left Hand Event Id`
   - `Right Hand Event Id`

## Suggested Event Mapping

- Use the left-hand event field for left-hand contact events.
- Use the right-hand event field for right-hand contact events.
- If both hands should feel the same pattern, you can reuse the same event ID in both fields.

## Fast Option

If you do not want to perform the manual integration, use the `with-bhaptics-sdk` branch.
That branch is meant to keep the official SDK pre-integrated for faster onboarding.

## Official References

- bHaptics Unity guide: https://docs.bhaptics.com/sdk/unity/guide
- bHaptics Developer Portal: https://developer.bhaptics.com/
