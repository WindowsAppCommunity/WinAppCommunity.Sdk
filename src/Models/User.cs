using Ipfs;
using System.Collections.Generic;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a user.
/// </summary>
public record User : IName
{
    /// <summary>
    /// The name of the user.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// An extended description of the user in markdown syntax.
    /// </summary>
    public required string ExtendedDescription { get; set; }

    /// <summary>
    /// Represents application connections added by the user.
    /// </summary>
    public Dictionary<string, DagCid> Connections { get; set; } = new();

    /// <summary>
    /// Represents links to external profiles or resources added by the user.
    /// </summary>
    public Link[] Links { get; set; } = [];

    /// <summary>
    /// A list of all the projects the user is registered on, along with their roles.
    /// </summary>
    public Dictionary<DagCid, Role> Projects { get; set; } = new();

    /// <summary>
    /// Represents all publishers the user is registered on, along with their roles.
    /// </summary>
    public Dictionary<DagCid, Role> Publishers { get; set; } = new();

    /// <summary>
    /// A flag that indicates whether the profile has requested to be forgotten.
    /// </summary>
    public bool? ForgetMe { get; set; }
}
