# Unity VR Volleyball Haptics Starter

Minimal Pico VR volleyball starter focused on one interaction loop:

- enter scene
- hand tracking drives the hand hit proxies
- volleyballs spawn continuously
- hitting the ball applies volleyball-specific motion rules
- optional bHaptics events can be triggered on contact

## Scene Behavior

`Assets/Scenes/SampleScene.unity` is the demo scene.

- `VolleyballSpawner` starts automatically on scene load.
- `VolleyballController` uses active hand hit proxies in the scene.
- bHaptics is optional. Leave the event IDs empty to disable playback.

## Key Scripts

- `Assets/scripts/VolleyballSpawner.cs`
- `Assets/scripts/VolleyballController.cs`
- `Assets/scripts/TrackerProxy.cs`
- `Assets/scripts/MyHandTrackOBJ.cs`

## bHaptics Setup

On the volleyball prefab / scene instance:

- enable `Enable Bhaptics`
- set `Left Hand Event Id`
- set `Right Hand Event Id`

If the fields are blank, the starter runs without bHaptics playback.

## Pico Dependency

This project currently references Pico XR as a local package path in `Packages/manifest.json`.
Before opening on another machine, replace that path with your local Pico XR SDK location.
