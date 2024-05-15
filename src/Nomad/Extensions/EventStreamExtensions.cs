using CommunityToolkit.Diagnostics;
using OwlCore.ComponentModel;
using OwlCore.ComponentModel.Nomad;
using OwlCore.Nomad.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WinAppCommunity.Sdk.Nomad.Extensions;

public static class EventStreamExtensions
{
    /// <summary>
    /// Resolves the full event stream from all sources organized by date, advancing all listening <see cref="ISharedEventStreamHandler{TContentPointer,TEventStreamSource,TEventStreamEntry}.ListeningEventStreamHandlers"/> on the given <paramref name="eventStreamHandler"/> using data from all available <see cref="ISources{T}.Sources"/>.
    /// </summary>
    public static async IAsyncEnumerable<TEventStreamEntry> AdvanceEventStreamToAtLeastAsync<TContentPointer, TEventStreamSource, TEventStreamEntry>(this ISharedEventStreamHandler<TContentPointer, TEventStreamSource, TEventStreamEntry> eventStreamHandler, DateTime maxDateTimeUtc, Func<TContentPointer, CancellationToken, Task<TEventStreamEntry>> contentPointerToStreamEntryAsync, [EnumeratorCancellation] CancellationToken cancellationToken)
        where TEventStreamSource : EventStream<TContentPointer>
        where TEventStreamEntry : EventStreamEntry<TContentPointer>
    {
        // Resolve all events in stream
        var resolvedEventStreamEntries = await eventStreamHandler.Sources.ResolveEventStreamsAsync(contentPointerToStreamEntryAsync, cancellationToken);

        // Playback event stream
        // Order event entries by oldest first
        foreach (var eventEntry in resolvedEventStreamEntries
                     .OrderBy(x => x.TimestampUtc)
                     .Where(x => (x.TimestampUtc ?? ThrowHelper.ThrowArgumentNullException<DateTime>()) <= maxDateTimeUtc))
        {
            // Advance event stream for all listening objects
            await eventStreamHandler.TryAdvanceEventStreamAsync(eventEntry, cancellationToken);
            yield return eventEntry;
        }
    }
}