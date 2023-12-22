using System.Text.Json.Serialization;
using Ipfs;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a single user.
/// </summary>
public record User
{
    /// <summary>
    /// Creates a new instance of <see cref="User"/>.
    /// </summary>
    [JsonConstructor]
    public User(string? name, string markdownAboutMe, ApplicationConnection[] connections, Link[] links, Cid[] projects, Cid[] publishers, bool? forgetMe)
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
    /// The name of the user.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// A summary of the user in markdown syntax.
    /// </summary>
    public string MarkdownAboutMe { get; set; }

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