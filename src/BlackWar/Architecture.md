# Black War Code Architecture

## Layers

- `GameModels.cs`: domain objects and data rules for weapons, vehicles, players, contracts, alliances, ranks, and the black market.
- `GameSystems.cs`: gameplay services for The Citadel, battle map events, match simulation, rewards, and demo bootstrapping.
- `Program.cs`: minimal entry point that runs a demo season.

## Porting to Unity

The model classes are intentionally engine-independent. In Unity, keep these classes as gameplay state and add MonoBehaviours for input, camera, physics, UI, audio, VFX, and networking. Dedicated servers should run the authoritative version of `MatchEngine` logic and reject invalid client actions.
