using Ipfs;
using OwlCore.Nomad;

namespace WinAppCommunity.Sdk.Nomad.Kubo;

/// <summary>
/// An event stream with event entry content stored on ipfs.
/// </summary>
public record KuboNomadEventStream : EventStream<Cid>;
