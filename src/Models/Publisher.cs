using Ipfs;
using System.Text.Json.Serialization;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents publisher data.
/// </summary>
public record Publisher
{
    /// <summary>
    /// Creates a new instance of <see cref="Publisher"/>.
    /// </summary>
    [JsonConstructor]
    public Publisher(string name, string description, string icon, string? accentColor, Link[] links, EmailConnection contactEmail, Cid[] projects, bool isPrivate)
    {
        Name = name;
        Description = description;
        Icon = icon;
        AccentColor = accentColor;
        Links = links;
        ContactEmail = contactEmail;
        Projects = projects;
        IsPrivate = isPrivate;
    }

    /// <summary>
    /// The name of the publisher.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A description of the publisher.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// An icon to represent this publisher.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// An hex-encoded accent color for this publisher.
    /// </summary>
    public string? AccentColor { get; set; }

    /// <summary>
    /// Represents links to external profiles or resources added by the publisher.
    /// </summary>
    public Link[] Links { get; set; }

    /// <summary>
    /// An <see cref="EmailConnection"/> that can be used to contact this publisher. 
    /// </summary>
    public EmailConnection ContactEmail { get; set; }

    /// <summary>
    /// A list of the projects published by this publisher.
    /// </summary>
    public Cid[] Projects { get; set; }

    /// <summary>
    /// A flag indicating whether this is a non-public project.
    /// </summary>
    public bool IsPrivate { get; set; }
}
