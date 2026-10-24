# The Singularity Workshop CiAi.CogE

**CogE projects lifecycle evidence for people and downstream automation.** It shows what is proven, what is missing, and where observations conflict; it never upgrades an unknown state into success.

## 01 — See it in a minute

Requires .NET 8:

```bash
dotnet run --project tests/TheSingularityWorkshop.CiAi.CogE.ContractTests --configuration Release
```

## 02 — Responsibility

CogE projects timestamped observations for an exact request and artifact version. Published, notified, retrieved, integrity-verified, locally validated, acknowledged, accepted, and reclaimed remain distinct facts. Out-of-order observations are sorted; mismatched identities are excluded and diagnosed. Missing prerequisites and contradictory evidence are reported, not silently repaired.

## 03 — Trust boundary

- Projection is reporting, not authorization or acceptance.
- A hash check does not prove semantic correctness.
- Reclamation is not inferred from acknowledgement.
- No network access, persistence, or execution.

## 04 — UML and use case

See [LifecycleProjector UML and use case](docs/UML/LifecycleProjectorUML.md).

## 05 — Release status

**Implementation readiness: 25%. Package preparation: 25%.** Typed lifecycle facts, deterministic projection, and contradiction diagnostics are committed; durable event ingestion, authenticated provenance, parent coordinator integration, and live UI/reporting remain outstanding. Not a publication approval.