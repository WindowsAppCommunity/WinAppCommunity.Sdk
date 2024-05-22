using Ipfs;
using OwlCore.Nomad;

namespace WinAppCommunity.Sdk.Nomad.Kubo;

/// <summary>
/// A modifiable kubo-based storage interface.
/// </summary>
public interface IModifiableNomadKuboEventStreamHandler<TEventEntryContent> : IModifiableSharedEventStreamHandler<TEventEntryContent, Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>, IReadOnlyNomadKuboEventStreamHandler<TEventEntryContent>
{
    /// <summary>
    /// The name of an Ipns key containing a Nomad event stream that can be appended and republished to modify the current folder.
    /// </summary>
    public string LocalEventStreamKeyName { get; init; }
}