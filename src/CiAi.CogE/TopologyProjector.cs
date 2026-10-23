namespace CiAi.CogE;

/// <summary>Projects supplied catalog and validated observations; never invents readiness or performs I/O.</summary>
public static class TopologyProjector
{
    public static TopologySnapshot Project(IEnumerable<RepositoryDescriptor> catalog, IEnumerable<OperationObservation> observations)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(observations);
        var repositories = catalog.ToArray();
        var nodes = new Dictionary<string, TopologyNode>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in repositories)
        {
            if (string.IsNullOrWhiteSpace(item.Repository) || string.IsNullOrWhiteSpace(item.DisplayName) || string.IsNullOrWhiteSpace(item.Role))
                throw new ArgumentException("Catalog entries require repository, display name, and role.", nameof(catalog));
            if (!nodes.TryAdd(item.Repository, new TopologyNode(item.Repository, item.DisplayName, item.Role, "unknown", "No validated operation evidence", null)))
                throw new ArgumentException("Catalog contains a duplicate repository identity.", nameof(catalog));
        }

        var edges = new List<TopologyEdge>();
        foreach (var observation in observations)
        {
            if (string.IsNullOrWhiteSpace(observation.RequestId) || string.IsNullOrWhiteSpace(observation.SourceRepository) ||
                string.IsNullOrWhiteSpace(observation.TargetRepository) || string.IsNullOrWhiteSpace(observation.OperationId))
                continue;
            if (!nodes.ContainsKey(observation.SourceRepository) || !nodes.ContainsKey(observation.TargetRepository))
                continue;

            var state = NormalizeState(observation.State, observation.EvidenceValidated);
            var evidenceUrl = observation.EvidenceValidated && IsHttpsUrl(observation.EvidenceUrl) ? observation.EvidenceUrl : null;
            var status = state is "unknown" or "stale"
                ? "Not verified; do not infer readiness"
                : string.IsNullOrWhiteSpace(observation.StatusText) ? state : observation.StatusText;
            var edgeId = observation.RequestId + ":" + observation.OperationId;
            edges.Add(new TopologyEdge(edgeId, observation.SourceRepository, observation.TargetRepository,
                observation.OperationId, state, status, evidenceUrl));

            if (state is "unknown" or "stale") continue;
            var target = nodes[observation.TargetRepository];
            nodes[observation.TargetRepository] = target with { State = state, StatusText = status, EvidenceUrl = evidenceUrl };
        }
        return new TopologySnapshot(nodes.Values.ToArray(), edges);
    }

    private static string NormalizeState(string state, bool evidenceValidated)
    {
        if (!evidenceValidated) return state == "stale" ? "stale" : "unknown";
        return state switch
        {
            "pending" => "pending",
            "succeeded" => "succeeded",
            "failed" => "failed",
            "rejected" => "rejected",
            "stale" => "stale",
            _ => "unknown"
        };
    }

    private static bool IsHttpsUrl(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps;
}
