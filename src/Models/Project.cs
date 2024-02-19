using Ipfs;
using System;
using System.Text.Json.Serialization;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents project data.
/// </summary>
public record Project
{
    /// <summary>
    /// Creates a new instance of <see cref="Project"/>.
    /// </summary>
    [JsonConstructor]
    public Project(Cid publisher, string name, string description, Cid icon, Cid heroImage, Cid[] images, string[] features, string? accentColor, string category, DateTime createdAt, Cid[] dependencies, Collaborator[] collaborators, Link[] links, bool? forgetMe, bool isPrivate)
    {
        Publisher = publisher;
        Name = name;
        Description = description;
        Icon = icon;
        HeroImage = heroImage;
        Images = images;
        Features = features;
        AccentColor = accentColor;
        Category = category;
        CreatedAt = createdAt;
        Dependencies = dependencies;
        Collaborators = collaborators;
        Links = links;
        ForgetMe = forgetMe;
        IsPrivate = isPrivate;
    }

    /// <summary>
    /// The publisher for this project.
    /// </summary>
    public Cid Publisher { get; set; }

    /// <summary>
    /// The name of this project.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A description of this project.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// A <see cref="Cid"/> pointing to a small icon for this project.
    /// </summary>
    public Cid Icon { get; set; }

    /// <summary>
    /// A <see cref="Cid"/> pointing to a banner or hero image for this project.
    /// </summary>
    public Cid HeroImage { get; set; }

    /// <summary>
    /// A list of <see cref="Cid"/>s that point to images demonstrating this project.
    /// </summary>
    public Cid[] Images { get; set; }

    /// <summary>
    /// A list of features provided by this project.
    /// </summary>
    public string[] Features { get; set; }

    /// <summary>
    /// An hex-encoded accent color for this publisher.
    /// </summary>
    public string? AccentColor { get; set; }

    /// <summary>
    /// The category defining this project, as found in an app store.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// The time this project was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Other projects which depend on this project.
    /// </summary>
    public Cid[] Dependencies { get; set; }

    /// <summary>
    /// The <see cref="User"/>s who collaborate on this project, and their corresponding roles.
    /// </summary>
    public Collaborator[] Collaborators { get; set; }

    /// <summary>
    /// Represents links to external profiles or resources added by the user.
    /// </summary>
    public Link[] Links { get; set; }

    /// <summary>
    /// A flag that indicates whether the profile has requested to be forgotten.
    /// </summary>
    public bool? ForgetMe { get; set; }

    /// <summary>
    /// A flag indicating whether this is a non-public project.
    /// </summary>
    public bool IsPrivate { get; set; }
}

// User: Draft a list of cli commands for modifying a project
// gpt4:
// 