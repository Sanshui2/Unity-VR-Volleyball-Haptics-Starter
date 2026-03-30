# Unity VR Volleyball Haptics Starter

Minimal Pico VR volleyball starter focused on one interaction loop:

- enter scene
- Pico hand tracking drives the hand hit proxies
- volleyballs spawn continuously
- hitting the ball works with general hand contact, with extra tuning for common volleyball motions
- optional bHaptics events can be triggered on contact

## Setup

- Open the project in Unity `2022.3.51f1c1`.
- `Packages/manifest.json` now points `com.unity.xr.picoxr` at the official Pico Unity Integration SDK Git repository instead of a machine-local path.
- The starter branch does not require the bHaptics Unity SDK to compile. Import the official bHaptics package only if you want playback.

## Branches

- `main`: clean starter branch with optional bHaptics hook points
- `with-bhaptics-sdk`: pre-integrated branch for people who want the official SDK already in place

## Scene Behavior

`Assets/Scenes/SampleScene.unity` is the demo scene.

- `VolleyballSpawner` starts automatically on scene load.
- `VolleyballController` uses active hand hit proxies in the scene.
- Motion-tracker calibration is intentionally not part of `main`; the starter stays focused on hand volleyball.
- bHaptics is optional. Leave the event IDs empty to disable playback.

## Key Scripts

- `Assets/scripts/VolleyballSpawner.cs`
- `Assets/scripts/VolleyballController.cs`
- `Assets/scripts/TrackerProxy.cs`
- `Assets/scripts/MyHandTrackOBJ.cs`
- `Assets/scripts/BhapticsBridge.cs`

## bHaptics Setup

On the volleyball prefab / scene instance:

- enable `Enable Bhaptics`
- set `Left Hand Event Id`
- set `Right Hand Event Id`

If the fields are blank, the starter runs without bHaptics playback.
If the bHaptics SDK is not installed, the starter still compiles and simply skips playback.

For the full manual integration flow on `main`, see `Docs/bhaptics-setup.md`.

## Pico Dependency

This project now references Pico XR through the official public Git package URL in `Packages/manifest.json`.
If you need a different Pico SDK revision, change that Git dependency to the version you want to pin.
