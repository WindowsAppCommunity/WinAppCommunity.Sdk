using Ipfs;
using System;
using System.Text.Json.Serialization;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a single managed user. 
/// </summary>
/// <param name="user"></param>
/// <param name="ipnsCid"></param>
public record ManagedUserMap(User user, Cid ipnsCid);

/// <summary>
/// Represents a single user.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public record User : IName
{
    /// <summary>
    /// Creates a new instance of <see cref="User"/>.
    /// </summary>
    [Newtonsoft.Json.JsonConstructor]
    public User(string name, string markdownAboutMe, ApplicationConnection[] connections, Link[] links, Cid[] projects, Cid[] publishers, bool? forgetMe)
    {
        Name = name;
        MarkdownAboutMe = markdownAboutMe;
        Connections = connections;
        Links = links;
        Projects = projects;
        Publishers = publishers;
        ForgetMe = forgetMe;
    }

    /// <summary>
    /// Creates a new instance of <see cref="User"/>.
    /// </summary>
    public User()
    {
        // Initialize properties with default values
        Name = string.Empty;
        MarkdownAboutMe = string.Empty;
        Connections = Array.Empty<ApplicationConnection>();
        Links = Array.Empty<Link>();
        Projects = Array.Empty<Cid>();
        Publishers = Array.Empty<Cid>();
        ForgetMe = null;
    }

    /// <summary>
    /// Creates a new instance of <see cref="User"/>.
    /// </summary>
    public User(string name, ApplicationConnection[] connections)
    {
        Name = name;
        MarkdownAboutMe = string.Empty;
        Connections = connections;
        Links = [];
        Projects = [];
        Publishers = [];
        ForgetMe = null;
    }

    /// <summary>
    /// The name of the user.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A summary of the user in markdown syntax.
    /// </summary>
    public string MarkdownAboutMe { get; set; }

    /// <summary>
    /// An icon to represent this user.
    /// </summary>
    public Cid? Icon { get; set; }

    /// <summary>
    /// Represents application connections added by the user.
    /// </summary>
    public ApplicationConnection[] Connections { get; set; }

    /// <summary>
    /// Represents links to external profiles or resources added by the user.
    /// </summary>
    public Link[] Links { get; set; }

    /// <summary>
    /// A list of all the projects the user is registered on.
    /// </summary>
    public Cid[] Projects { get; set; }

    /// <summary>
    /// Represents all publishers the user is registered on.
    /// </summary>
    public Cid[] Publishers { get; set; }

    /// <summary>
    /// A flag that indicates whether the profile has requested to be forgotten.
    /// </summary>
    public bool? ForgetMe { get; set; }
}