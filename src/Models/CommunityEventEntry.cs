using Ipfs;
using Newtonsoft.Json;
using WinAppCommunity.Sdk.Models.JsonConverters;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a single entry in a community event.
/// </summary>
[JsonConverter(typeof(ApplicationConnectionJsonConverter))]
public record CommunityEventEntry
{
    /// <summary>
    /// Creates an new instance of <see cref="CommunityEventEntry"/>.
    /// </summary>
    [JsonConstructor]
    public CommunityEventEntry(Cid @event, Cid project)
    {
        @Event = @event;
        Project = project;
    }

    /// <summary>
    /// A <see cref="Cid"/> pointing to a <see cref="CommunityEvent"/>.
    /// </summary>
    public Cid @Event { get; set; }

    /// <summary>
    /// A <see cref="Cid"/> pointing to the <see cref="Models.Project"/> that participated in this event.
    /// </summary>
    public Cid Project { get; set; }
}