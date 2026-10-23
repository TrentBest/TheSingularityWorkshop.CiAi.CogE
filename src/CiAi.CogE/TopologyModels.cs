namespace CiAi.CogE;

public sealed record RepositoryDescriptor(string Repository, string DisplayName, string Role);

/// <summary>Evidence state already normalized by the trusted host; this projection does not fetch or validate CI.</summary>
public sealed record OperationObservation(
    string RequestId,
    string SourceRepository,
    string TargetRepository,
    string OperationId,
    string State,
    string StatusText,
    string? EvidenceUrl,
    bool EvidenceValidated);

public sealed record TopologyNode(string Id, string Label, string Role, string State, string StatusText, string? EvidenceUrl);
public sealed record TopologyEdge(string Id, string SourceNodeId, string TargetNodeId, string Label, string State, string StatusText, string? EvidenceUrl);
public sealed record TopologySnapshot(IReadOnlyList<TopologyNode> Nodes, IReadOnlyList<TopologyEdge> Edges);
