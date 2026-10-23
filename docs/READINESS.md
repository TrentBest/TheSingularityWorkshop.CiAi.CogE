# CogE Readiness

**Readiness: 20% — presentation-neutral projection prototype, not connected to GUI or live CI.**

## Implemented on development
- Repository catalog descriptors and topology node/edge/snapshot contracts.
- Deterministic projection from supplied observations to directed edges.
- Unknown/unvalidated claims become unknown; unvalidated evidence URLs are omitted.
- Stale observations never promote target node readiness.
- Duplicate repository identities are rejected; observations referencing absent nodes are omitted.
- Offline checks cover validated, unvalidated, stale, and duplicate-catalog cases.

## Not implemented
- GUI.SVG/GUI.Blazor rendering or event wiring.
- Live repository catalog loading or authoritative GitHub run verification.
- Shared compiled reference to CogB/CogC contracts.
- Layout, graph diffing, history, streaming updates, or persistence.

## Run
    dotnet run --project tests/CiAi.CogE.Tests/CiAi.CogE.Tests.csproj --configuration Release

The projector is a pure view-model transformation. It does not make policy decisions and trusts the host's EvidenceValidated flag only as an input contract; CiAi must set that flag only after authoritative verification.
