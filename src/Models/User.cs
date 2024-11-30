using Ipfs;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a user.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public record User : IName
{
    /// <summary>
    /// The name of the user.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// A summary of the user in markdown syntax.
    /// </summary>
    public required string MarkdownAboutMe { get; set; }

    /// <summary>
    /// An icon to represent this user.
    /// </summary>
    public Cid? Icon { get; set; }

    /// <summary>
    /// Represents application connections added by the user.
    /// </summary>
    public Dictionary<string, DagCid> Connections { get; set; } = [];

    /// <summary>
    /// Represents links to external profiles or resources added by the user.
    /// </summary>
    public Link[] Links { get; set; } = [];

    /// <summary>
    /// A list of all the projects the user is registered on.
    /// </summary>
    public Cid[] Projects { get; set; } = [];

    /// <summary>
    /// Represents all publishers the user is registered on.
    /// </summary>
    public Cid[] Publishers { get; set; } = [];

    /// <summary>
    /// A flag that indicates whether the profile has requested to be forgotten.
    /// </summary>
    public bool? ForgetMe { get; set; }
}