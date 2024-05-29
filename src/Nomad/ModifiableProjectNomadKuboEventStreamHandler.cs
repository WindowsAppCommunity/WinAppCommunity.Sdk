using Ipfs;
using OwlCore.Nomad;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinAppCommunity.Sdk.Nomad.Kubo;
using WinAppCommunity.Sdk.Nomad.Kubo.Extensions;
using WinAppCommunity.Sdk.Nomad.UpdateEvents;

namespace WinAppCommunity.Sdk.Nomad;

/// <summary>
/// A Nomad event stream handler for projects.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="ModifiableProjectNomadKuboEventStreamHandler"/>.
/// </remarks>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>. </param>
public class ModifiableProjectNomadKuboEventStreamHandler(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ReadOnlyProjectNomadKuboEventStreamHandler(listeningEventStreamHandlers), IModifiableNomadKuboEventStreamHandler<ProjectUpdateEvent>
{
    /// <inheritdoc />
    public required string RoamingKeyName { get; init; }
    
    /// <inheritdoc />
    public async Task AppendNewEntryAsync(ProjectUpdateEvent updateEvent, CancellationToken cancellationToken = default)
    {
        await this.AppendNewEntryAsync(updateEvent, KuboOptions.IpnsLifetime, () => new KuboNomadEventStream { Entries = [], Id = Id, Label = Inner.Name, }, cancellationToken);
    }
}