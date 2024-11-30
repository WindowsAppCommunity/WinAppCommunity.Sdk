using Ipfs;
using Newtonsoft.Json;
using OwlCore.ComponentModel;
using WinAppCommunity.Sdk.Models.JsonConverters;

namespace WinAppCommunity.Sdk.Models.UpdateEvents;

[JsonConverter(typeof(UpdateEventJsonConverter))]
public abstract record WinAppCommunityUpdateEvent(string Id, string EventId) : IHasId;

public record ConnectionAddEvent(string Id, DagCid Value) : WinAppCommunityUpdateEvent(Id, nameof(ConnectionAddEvent));

public record ConnectionRemoveEvent(string Id, DagCid Value) : WinAppCommunityUpdateEvent(Id, nameof(ConnectionRemoveEvent));