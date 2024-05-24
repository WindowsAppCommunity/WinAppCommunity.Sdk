using Ipfs;
using Newtonsoft.Json;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a content publisher.
/// </summary>
public record Publisher : IName
{
    /// <summary>
    /// Creates a new instance of <see cref="Publisher"/>.
    /// </summary>
    [JsonConstructor]
    public Publisher(string name, string description, Cid owner, Cid? icon, string? accentColor, EmailConnection? contactEmail = null)
    {
        Name = name;
        Description = description;
        Icon = icon;
        Owner = owner;
        AccentColor = accentColor;
        ContactEmail = contactEmail;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Publisher"/>.
    /// </summary>
    public Publisher()
    : this(string.Empty, string.Empty, string.Empty, default, default)
    {
    }

    /// <summary>
    /// The Cid of the <see cref="User"/> who owns this publisher.
    /// </summary>
    public Cid Owner { get; set; }

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
    public Cid? Icon { get; set; }

    /// <summary>
    /// A hex-encoded accent color for this publisher.
    /// </summary>
    public string? AccentColor { get; set; }

    /// <summary>
    /// Represents links to external profiles or resources added by the publisher.
    /// </summary>
    public Link[] Links { get; set; } = [];

    /// <summary>
    /// A <see cref="EmailConnection"/> that can be used to contact this publisher. 
    /// </summary>
    public EmailConnection? ContactEmail { get; set; }

    /// <summary>
    /// A list of the projects registered with this publisher.
    /// </summary>
    public Cid[] Projects { get; set; } = [];

    /// <summary>
    /// Users who are registered to participate in this publisher.
    /// </summary>
    public Cid[] Users { get; set; } = [];

    /// <summary>
    /// A list of other publishers who are managed under this publisher.
    /// </summary>
    public Cid[] ParentPublishers { get; set; } = [];

    /// <summary>
    /// A list of other publishers who are managed under this publisher.
    /// </summary>
    public Cid[] ChildPublishers { get; set; } = [];

    /// <summary>
    /// A flag indicating whether this is a non-public project.
    /// </summary>
    public bool IsPrivate { get; set; }
}