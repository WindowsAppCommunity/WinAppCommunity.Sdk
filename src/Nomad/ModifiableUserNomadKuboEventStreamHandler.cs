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
/// A Nomad event stream handler for users.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="ModifiableUserNomadKuboEventStreamHandler"/>.
/// </remarks>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>. </param>
public class ModifiableUserNomadKuboEventStreamHandler(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ReadOnlyUserNomadKuboEventStreamHandler(listeningEventStreamHandlers), IModifiableNomadKuboEventStreamHandler<UserUpdateEvent>
{
    /// <inheritdoc />
    public bool ShouldPin { get; set; } = true;

    /// <inheritdoc />
    public required TimeSpan IpnsLifetime { get; set; }

    /// <inheritdoc />
    public required string LocalEventStreamKeyName { get; init; }

    /// <inheritdoc />
    public async Task AppendNewEntryAsync(UserUpdateEvent updateEvent, CancellationToken cancellationToken = default)
    {
        // Use extension method
        await this.AppendNewEntryAsync(updateEvent, IpnsLifetime, () => new KuboNomadEventStream { Entries = [], Id = Id, Label = Inner.Name, }, cancellationToken);
    }
}