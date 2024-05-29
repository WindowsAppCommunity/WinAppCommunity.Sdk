using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Storage;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk;

/// <summary>
/// Represents a project.
/// </summary>
public interface IReadOnlyProject : IHasId
{
    /// <summary>
    /// The name of this project.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// A description of this project.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// A list of features provided by this project.
    /// </summary>
    public string[] Features { get; }

    /// <summary>
    /// A hex-encoded accent color for this publisher.
    /// </summary>
    public string? AccentColor { get; }

    /// <summary>
    /// The category defining this project, as found in an app store.
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// The time this project was created.
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Represents links to external profiles or resources added by the user.
    /// </summary>
    public Link[] Links { get; }

    /// <summary>
    /// A flag that indicates whether the profile has requested to be forgotten.
    /// </summary>
    public bool? ForgetMe { get; }

    /// <summary>
    /// A flag indicating whether this is a non-public project.
    /// </summary>
    public bool IsPrivate { get; }

    /// <summary>
    /// Holds information about project connections that have been published for consumption by an end user, such as a Microsoft Store app, a package on nuget.org, a git repo, etc.
    /// </summary>
    public ApplicationConnection[] Connections { get; }
    
    /// <summary>
    /// Raised when <see cref="Name"/> is updated.
    /// </summary>
    public event EventHandler<string>? NameUpdated;
    
    /// <summary>
    /// Raised when <see cref="Description"/> is updated.
    /// </summary>
    public event EventHandler<string>? DescriptionUpdated;
    
    /// <summary>
    /// Raised when <see cref="Features"/> is updated.
    /// </summary>
    public event EventHandler<string[]>? FeaturesUpdated;
    
    /// <summary>
    /// Raised when <see cref="AccentColor"/> is updated.
    /// </summary>
    public event EventHandler<string?>? AccentColorUpdated;
    
    /// <summary>
    /// Raised when <see cref="Category"/> is updated.
    /// </summary>
    public event EventHandler<string>? CategoryUpdated;
    
    /// <summary>
    /// Raised when <see cref="CreatedAt"/> is updated.
    /// </summary>
    public event EventHandler<DateTime>? CreatedAtUpdated;
    
    /// <summary>
    /// Raised when <see cref="Links"/> is updated.
    /// </summary>
    public event EventHandler<Link[]>? LinksUpdated;
    
    /// <summary>
    /// Raised when <see cref="ForgetMe"/> is updated.
    /// </summary>
    public event EventHandler<bool?>? ForgetMeUpdated;
    
    /// <summary>
    /// Raised when <see cref="IsPrivate"/> is updated.
    /// </summary>
    public event EventHandler<bool>? IsPrivateUpdated;
    
    /// <summary>
    /// Raised when <see cref="Connections"/> is updated.
    /// </summary>
    public event EventHandler<ApplicationConnection[]>? ConnectionsUpdated;

    /// <summary>
    /// Gets the publisher for this project.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task<IReadOnlyPublisher> GetPublisherAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the image files for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the icon file for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task<IFile?> GetIconFileAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a file containing the hero image for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task<IFile?> GetHeroImageFileAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the collaborators for ths project.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<(IReadOnlyUser User, Role Role)> GetCollaboratorsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the dependencies for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IReadOnlyProject> GetDependenciesAsync(CancellationToken cancellationToken);
}