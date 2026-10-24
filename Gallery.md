# CogE Gallery

CogE is a small, dependency-free CiAi control-plane component. It makes one responsibility explicit and testable instead of hiding policy, identity, correlation, or lifecycle assumptions in coordinator code.

## See it in a minute

```bash
dotnet run --project tests/TheSingularityWorkshop.CiAi.CogE.ContractTests --configuration Release
```

## Recipe

- [LifecycleProjector UML and use case](docs/UML/LifecycleProjectorUML.md)
- [LifecycleProjector class companion](src/TheSingularityWorkshop.CiAi.CogE/LifecycleProjector.md)
- [CI contract verification](https://github.com/TrentBest/TheSingularityWorkshop.CiAi.CogE/actions)

This is a contract demonstration, not a claim of end-to-end CiAi orchestration.
