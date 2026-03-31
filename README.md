# Unity VR Volleyball Haptics Starter

Minimal Pico VR volleyball starter focused on one interaction loop:

- enter scene
- Pico Motion Trackers drive the hand hit proxies after auto-calibration
- volleyballs spawn continuously
- hitting the ball works with general hand contact, with extra tuning for common volleyball motions
- optional bHaptics events can be triggered on contact

## Setup

- Open the project in Unity `2022.3.51f1c1`.
- `Packages/manifest.json` points `com.unity.xr.picoxr` at the official Pico Unity Integration SDK Git repository instead of a machine-local path.
- Pair two Pico Motion Trackers with the headset before testing this branch.
- The tracker-enhanced branch does not require the bHaptics Unity SDK to compile. Import the official bHaptics package only if you want playback.

## Branches

- `main`: clean starter branch with Pico hand tracking and optional bHaptics hook points
- `tracker-enhanced`: this branch, with Pico Motion Tracker connection and left/right auto-calibration
- `with-bhaptics-sdk`: pre-integrated branch for people who want the official SDK already in place

## Scene Behavior

`Assets/Scenes/SampleScene.unity` is the demo scene.

- `VolleyballSpawner` starts automatically on scene load.
- `PlayerInputManager` requests two Pico Motion Trackers and assigns left and right when they are held apart in front of the headset.
- `VolleyballController` uses the tracker-driven hit proxies in the scene.
- bHaptics is optional. Leave the event IDs empty to disable playback.

## Key Scripts

- `Assets/scripts/PlayerInputManager.cs`
- `Assets/scripts/TrackerProxy.cs`
- `Assets/scripts/VolleyballController.cs`
- `Assets/scripts/VolleyballSpawner.cs`
- `Assets/scripts/MotionTrackerManager.cs`
- `Assets/scripts/BhapticsBridge.cs`

## Tracker Setup

- Open `Assets/Scenes/SampleScene.unity`.
- Enter Play Mode on device.
- Hold the two Motion Trackers apart in front of the headset until the left/right assignment finishes.
- Volleyballs spawn immediately, so you can start hitting as soon as calibration completes.

## bHaptics Setup

On the volleyball prefab / scene instance:

- enable `Enable Bhaptics`
- set `Left Hand Event Id`
- set `Right Hand Event Id`

If the fields are blank, the starter runs without bHaptics playback.
If the bHaptics SDK is not installed, the starter still compiles and simply skips playback.

For the full manual integration flow on `main`, see `Docs/bhaptics-setup.md`.
On `with-bhaptics-sdk`, you can skip the SDK import step and go straight to event ID configuration.

## Pico Dependency

This project references Pico XR through the official public Git package URL in `Packages/manifest.json`.
If you need a different Pico SDK revision, change that Git dependency to the version you want to pin.
