using Ipfs;
using Newtonsoft.Json;
using OwlCore.ComponentModel;
using WinAppCommunity.Sdk.Models.JsonConverters;

namespace WinAppCommunity.Sdk.Models.UpdateEvents;

/// <summary>
/// Represents an update event in the Windows App Community.
/// </summary>
/// <param name="Id">The unique identifier of the event.</param>
/// <param name="EventId">The unique identifier of the event type.</param>
[JsonConverter(typeof(UpdateEventJsonConverter))]
public abstract record WinAppCommunityUpdateEvent(string Id, string EventId) : IHasId;

/// <summary>
/// Represents an event where a connection is added.
/// </summary>
/// <param name="Id">The unique identifier of the event.</param>
/// <param name="Value">The value of the connection to be added.</param>
public record ConnectionAddEvent(string Id, DagCid Value) : WinAppCommunityUpdateEvent(Id, nameof(ConnectionAddEvent));

/// <summary>
/// Represents an event where a connection is removed.
/// </summary>
/// <param name="Id">The unique identifier of the event.</param>
/// <param name="Value">The value of the connection to be removed.</param>
public record ConnectionRemoveEvent(string Id, DagCid Value) : WinAppCommunityUpdateEvent(Id, nameof(ConnectionRemoveEvent));
