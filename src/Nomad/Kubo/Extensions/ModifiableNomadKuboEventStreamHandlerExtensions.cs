using CommunityToolkit.Diagnostics;
using OwlCore.Kubo;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WinAppCommunity.Sdk.Nomad.Kubo.Extensions;

/// <summary>
/// Extension methods for <see cref="IModifiableNomadKuboEventStreamHandler{TEventEntryContent}"/>.
/// </summary>
public static class ModifiableNomadKuboEventStreamHandlerExtensions
{
    /// <summary>
    /// Appends a new event entry to the provided modifiable event stream handler.
    /// </summary>
    /// <param name="eventStreamHandler">The event stream handler to append the provided <paramref name="updateEvent"/> to.</param>
    /// <param name="updateEvent">The update event to append.</param>
    /// <param name="ipnsLifetime">The ipns lifetime for the published key to be valid for. Node must be online at least once per this interval of time for the published ipns key to stay in the dht.</param>
    /// <param name="getDefaultEventStream">Gets the default event stream type when needed for creation.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public static async Task AppendNewEntryAsync<TEventEntryContent>(this IModifiableNomadKuboEventStreamHandler<TEventEntryContent> eventStreamHandler, TEventEntryContent updateEvent, TimeSpan ipnsLifetime, Func<KuboNomadEventStream> getDefaultEventStream, CancellationToken cancellationToken)
        where TEventEntryContent : notnull
    {
        // Get or create event stream on ipns
        var key = await eventStreamHandler.Client.GetOrCreateKeyAsync(eventStreamHandler.LocalEventStreamKeyName, _ => getDefaultEventStream(), ipnsLifetime, cancellationToken: cancellationToken);
        Guard.IsNotNull(key);

        var (eventStream, _) = await eventStreamHandler.Client.ResolveDagCidAsync<KuboNomadEventStream>(key.Id, nocache: !eventStreamHandler.UseCache, cancellationToken);
        Guard.IsNotNull(eventStream);

        var updateEventDagCid = await eventStreamHandler.Client.Dag.PutAsync(updateEvent, pin: eventStreamHandler.ShouldPin, cancel: cancellationToken);

        // Create new nomad event stream entry
        var newEventStreamEntry = new KuboNomadEventStreamEntry
        {
            Id = eventStreamHandler.Id,
            TimestampUtc = DateTime.UtcNow,
            Content = updateEventDagCid,
            PriorEventStreamEntry = null, // TODO: remove this
        };

        var newEventStreamEntryDagCid = await eventStreamHandler.Client.Dag.PutAsync(newEventStreamEntry, pin: eventStreamHandler.ShouldPin, cancel: cancellationToken);

        // Add new event to event stream
        eventStream.Entries.Add(newEventStreamEntryDagCid);

        var updatedEventStreamDagCid = await eventStreamHandler.Client.Dag.PutAsync(eventStream, pin: eventStreamHandler.ShouldPin, cancel: cancellationToken);

        // Publish updated event stream
        _ = await eventStreamHandler.Client.Name.PublishAsync($"/ipfs/{updatedEventStreamDagCid}", key: eventStreamHandler.LocalEventStreamKeyName, lifetime: eventStreamHandler.IpnsLifetime, cancellationToken);
    }
}