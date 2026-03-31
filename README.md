# Unity VR Volleyball Haptics Starter

Minimal Unity starter for VR volleyball interactions on Pico.

This repository is intentionally scoped to one reusable loop:

- open the demo scene
- spawn volleyballs continuously
- hit the ball with hand-driven colliders
- optionally trigger bHaptics events on contact

The original experiment UI, scoring flow, and trial-management logic are not part of this starter.

## Quick Start

- Open the project in Unity `2022.3.51f1c1`.
- Open `Assets/Scenes/SampleScene.unity`.
- Enter Play Mode or build to device.
- The scene starts spawning volleyballs automatically.

## Branches

- `main`: clean starter branch using Pico hand tracking and optional bHaptics hook points
- `tracker-enhanced`: Pico Motion Tracker branch with left/right auto-calibration
- `with-bhaptics-sdk`: branch with the official bHaptics Unity SDK already restored in the project

## What Is Included

- continuous volleyball spawning
- volleyball-specific hit tuning for common motions such as bump and underhand serve
- reusable hand hit proxies
- optional bHaptics bridge layer
- a minimal demo scene without experiment UI

## bHaptics

This repository does not require the bHaptics Unity SDK to compile on `main`.

To enable playback:

- import the official bHaptics Unity SDK, or use `with-bhaptics-sdk`
- enable `Enable Bhaptics` on the volleyball prefab / scene instance
- set `Left Hand Event Id`
- set `Right Hand Event Id`

If the event IDs are blank, the project still runs and simply skips playback.

For the manual setup flow, see `Docs/bhaptics-setup.md`.

## Pico Dependency

`Packages/manifest.json` references Pico XR through the official public Git package URL instead of a machine-local path.

If you need a different Pico SDK revision, update that Git dependency to the version you want to pin.

## License

The original project code and starter-specific scene wiring in this repository are released under the MIT License. See `LICENSE`.

Third-party SDKs, packages, fonts, models, textures, and other bundled external resources remain under their own original licenses. This repository does not relicense Pico SDK content, bHaptics SDK content, or any other third-party assets.
