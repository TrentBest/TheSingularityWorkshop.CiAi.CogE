using CiAi.CogE;

var catalog = new[]
{
    new RepositoryDescriptor("TrentBest/CiAi", "CiAi", "Coordinator"),
    new RepositoryDescriptor("TrentBest/HeadlessAi", "HeadlessAi", "Provider-neutral HTTP adapter")
};
var snapshot = TopologyProjector.Project(catalog, new[]
{
    new OperationObservation("req-1", "TrentBest/CiAi", "TrentBest/HeadlessAi", "observe.status", "succeeded", "Verified run succeeded", "https://github.com/TrentBest/HeadlessAi/actions/runs/123", true),
    new OperationObservation("req-2", "TrentBest/CiAi", "TrentBest/HeadlessAi", "publish.package", "succeeded", "Untrusted claimed success", "https://example.com/fake", false),
    new OperationObservation("req-3", "TrentBest/CiAi", "TrentBest/HeadlessAi", "observe.status", "stale", "Old observation", "https://github.com/TrentBest/HeadlessAi/actions/runs/99", true)
});
Require(snapshot.Nodes.Count == 2, "catalog nodes should be projected");
Require(snapshot.Edges.Count == 3, "each known-source/target observation should project an edge");
Require(snapshot.Edges[0].State == "succeeded", "validated known state should be retained");
Require(snapshot.Edges[0].EvidenceUrl is not null, "validated HTTPS evidence link should be projected");
Require(snapshot.Edges[1].State == "unknown" && snapshot.Edges[1].EvidenceUrl is null, "unvalidated claims must become unknown and lose evidence link");
Require(snapshot.Edges[2].State == "stale" && snapshot.Edges[2].StatusText.Contains("Not verified"), "stale evidence must not imply readiness");
var untrustedOnly = TopologyProjector.Project(catalog, new[] { new OperationObservation("req-2", "TrentBest/CiAi", "TrentBest/HeadlessAi", "publish.package", "succeeded", "Untrusted claimed success", "https://example.com/fake", false), new OperationObservation("req-3", "TrentBest/CiAi", "TrentBest/HeadlessAi", "observe.status", "stale", "Old observation", "https://github.com/TrentBest/HeadlessAi/actions/runs/99", true) });
Require(untrustedOnly.Nodes.Single(n => n.Id == "TrentBest/HeadlessAi").State == "unknown", "stale/unvalidated observations alone must not promote node readiness");
var duplicateRejected = false;
try { TopologyProjector.Project(catalog.Concat(new[] { catalog[0] }), Array.Empty<OperationObservation>()); }
catch (ArgumentException) { duplicateRejected = true; }
Require(duplicateRejected, "duplicate catalog identities must be rejected");
Console.WriteLine("PASS: CogE topology projection checks");
return;

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
