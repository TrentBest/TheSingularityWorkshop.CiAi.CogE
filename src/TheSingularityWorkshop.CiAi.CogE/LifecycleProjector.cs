namespace TheSingularityWorkshop.CiAi.CogE;

/// <summary>Independent lifecycle facts. These are not interchangeable success states.</summary>
public enum LifecycleFact
{
    Requested, Prepared, Published, Notified, Retrieved, IntegrityVerified,
    LocallyValidated, Acknowledged, Accepted, Reclaimed, Failed, Partial, Cancelled, Expired
}

/// <summary>A timestamped observation about one exact request and artifact version.</summary>
public sealed record LifecycleObservation
{
    public LifecycleObservation(string requestId, string artifactId, string artifactVersion,
        LifecycleFact fact, DateTimeOffset observedAt, string sourceIdentity, string? digest = null)
    {
        RequestId = Token(requestId, nameof(requestId));
        ArtifactId = Token(artifactId, nameof(artifactId));
        ArtifactVersion = Token(artifactVersion, nameof(artifactVersion));
        SourceIdentity = Token(sourceIdentity, nameof(sourceIdentity));
        if (digest is not null && (digest.Length != 64 || !digest.All(Uri.IsHexDigit)))
            throw new ArgumentException("Digest must be a full SHA-256 hexadecimal value.", nameof(digest));
        if (!Enum.IsDefined(fact)) throw new ArgumentOutOfRangeException(nameof(fact));
        Fact = fact;
        ObservedAt = observedAt;
        Digest = digest?.ToLowerInvariant();
    }
    public string RequestId { get; }
    public string ArtifactId { get; }
    public string ArtifactVersion { get; }
    public LifecycleFact Fact { get; }
    public DateTimeOffset ObservedAt { get; }
    public string SourceIdentity { get; }
    public string? Digest { get; }

    private static string Token(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 128 || value.Any(char.IsControl))
            throw new ArgumentException("Value must be a bounded, non-empty identifier.", parameter);
        return value;
    }
}

/// <summary>Read-only projection of facts, evidence order, and contradictions for one artifact identity.</summary>
public sealed class LifecycleProjection
{
    internal LifecycleProjection(string requestId, string artifactId, string artifactVersion,
        IReadOnlyList<LifecycleObservation> observations, IReadOnlyList<string> diagnostics)
    {
        RequestId = requestId;
        ArtifactId = artifactId;
        ArtifactVersion = artifactVersion;
        Observations = observations;
        Diagnostics = diagnostics;
        Facts = Array.AsReadOnly(observations.Select(x => x.Fact).Distinct().ToArray());
    }
    public string RequestId { get; }
    public string ArtifactId { get; }
    public string ArtifactVersion { get; }
    public IReadOnlyList<LifecycleObservation> Observations { get; }
    public IReadOnlyList<LifecycleFact> Facts { get; }
    public IReadOnlyList<string> Diagnostics { get; }
    public bool Has(LifecycleFact fact) => Facts.Contains(fact);
}

/// <summary>Projects evidence without inventing missing lifecycle steps or upgrading uncertain outcomes.</summary>
public static class LifecycleProjector
{
    public static LifecycleProjection Project(string requestId, string artifactId, string artifactVersion,
        IEnumerable<LifecycleObservation> observations)
    {
        ArgumentNullException.ThrowIfNull(observations);
        var request = RequireIdentity(requestId, nameof(requestId));
        var artifact = RequireIdentity(artifactId, nameof(artifactId));
        var version = RequireIdentity(artifactVersion, nameof(artifactVersion));
        var accepted = new List<LifecycleObservation>();
        var diagnostics = new List<string>();

        foreach (var observation in observations)
        {
            if (observation is null) { diagnostics.Add("observation.null"); continue; }
            if (observation.RequestId != request) { diagnostics.Add("requestId.mismatch"); continue; }
            if (observation.ArtifactId != artifact) { diagnostics.Add("artifactId.mismatch"); continue; }
            if (observation.ArtifactVersion != version) { diagnostics.Add("artifactVersion.mismatch"); continue; }
            accepted.Add(observation);
        }

        var ordered = accepted.OrderBy(x => x.ObservedAt).ThenBy(x => x.Fact).ThenBy(x => x.SourceIdentity, StringComparer.Ordinal).ToArray();
        var facts = ordered.Select(x => x.Fact).ToHashSet();
        if (facts.Contains(LifecycleFact.IntegrityVerified) && !facts.Contains(LifecycleFact.Retrieved))
            diagnostics.Add("integrity-without-retrieval");
        if (facts.Contains(LifecycleFact.Accepted) && !facts.Contains(LifecycleFact.LocallyValidated))
            diagnostics.Add("accepted-without-local-validation");
        if (facts.Contains(LifecycleFact.Reclaimed) && !facts.Contains(LifecycleFact.Acknowledged))
            diagnostics.Add("reclaimed-without-acknowledgement");
        if (facts.Contains(LifecycleFact.Failed) && facts.Contains(LifecycleFact.Accepted))
            diagnostics.Add("accepted-and-failed");
        return new LifecycleProjection(request, artifact, version, Array.AsReadOnly(ordered), Array.AsReadOnly(diagnostics.Distinct().ToArray()));
    }

    private static string RequireIdentity(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 128 || value.Any(char.IsControl))
            throw new ArgumentException("Identity must be bounded and non-empty.", parameter);
        return value;
    }
}
