using System;
using Newtonsoft.Json;
using Ipfs;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a single entry in a community event.
/// </summary>
public record CommunityEvent : IName
{
    /// <summary>
    /// Creates a new instance of <see cref="CommunityEvent"/>.
    /// </summary>
    [JsonConstructor]
    public CommunityEvent(string name, Cid[] entries, DateTime eventDateTime)
    {
        Name = name;
        Entries = entries;
        EventDateTime = eventDateTime;
    }

    /// <summary>
    /// The name of this event.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A list of <see cref="Cid"/>s pointing to <see cref="CommunityEventEntry"/> for this event.
    /// </summary>
    public Cid[] Entries { get; init; }
    
    /// <summary>
    /// The time/date of the event.
    /// </summary>
    public DateTime EventDateTime { get; set; }
}