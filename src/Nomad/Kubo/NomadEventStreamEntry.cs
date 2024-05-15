using Ipfs;
using OwlCore.ComponentModel.Nomad;

namespace WinAppCommunity.Sdk.Nomad.Kubo;

/// <summary>
/// Represents a single entry in an event stream and the data needed to reconstruct it.
/// </summary>
public record KuboNomadEventStreamEntry : LinkedEventStreamEntry<Cid>
{
}
