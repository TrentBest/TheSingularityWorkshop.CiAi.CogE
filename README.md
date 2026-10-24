# CiAi.CogE — GUI-Ready Topology Projection

**Mission:** transform a repository catalog and host-supplied operation observations into a presentation-neutral directed graph model. CogE is not a GUI framework and does not fetch CI state.

## Documentation

See [TopologyProjector UML and use case](docs/UML/TopologyProjectorUML.md) for the projection model and evidence trust boundary.

## 01 — See it in a minute

Requires the .NET 8 SDK.

    dotnet build src/CiAi.CogE/CiAi.CogE.csproj --configuration Release
    dotnet run --project tests/CiAi.CogE.Tests/CiAi.CogE.Tests.csproj --configuration Release

Offline checks cover known catalog nodes, directional edges, validated evidence links, unvalidated claims, stale observations, and duplicate catalog identities.

## 02 — Projection contract

The input catalog supplies repository identity, display label, and role. Each operation observation supplies request identity, source/target, operation, state, status text, evidence URL, and an explicit host-set EvidenceValidated flag. Unknown or unvalidated claims project as unknown; unvalidated evidence links are omitted. Stale evidence is labeled not verified and does not itself promote a node's readiness.

## 03 — Trust boundary

CogE performs no network calls and does not verify GitHub runs. It cannot independently establish that EvidenceValidated is truthful: only CiAi should set that flag after fetching and validating authoritative provider data. GUI.SVG/GUI.Blazor rendering, live catalog loading, shared CogB/CogC types, graph history, and event wiring remain future integration work.

## 04 — Status

**Readiness: 20%.** Pure projection prototype with offline checks; not connected to the GUI or live CI. See [READINESS.md](docs/READINESS.md).
