using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Storage;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk;

/// <summary>
/// Represents a publisher, a collection of projects and users who publishes content to users.
/// </summary>
public interface IReadOnlyPublisher : IHasId
{
    /// <summary>
    /// The name of the publisher.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// A description of the publisher.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// A hex-encoded accent color for this publisher.
    /// </summary>
    public string? AccentColor { get; }

    /// <summary>
    /// Represents links to external profiles or resources added by the publisher.
    /// </summary>
    public Link[] Links { get; }

    /// <summary>
    /// A <see cref="EmailConnection"/> that can be used to contact this publisher. 
    /// </summary>
    public EmailConnection? ContactEmail { get; }

    /// <summary>
    /// A flag indicating whether this is a non-public project.
    /// </summary>
    public bool IsPrivate { get; }
    
    /// <summary>
    /// Raised when <see cref="Name"/> is updated.
    /// </summary>
    public event EventHandler<string>? NameUpdated;
    
    /// <summary>
    /// Raised when <see cref="Description"/> is updated.
    /// </summary>
    public event EventHandler<string>? DescriptionUpdated;
    
    /// <summary>
    /// Raised when <see cref="AccentColor"/> is updated.
    /// </summary>
    public event EventHandler<string?>? AccentColorUpdated;
    
    /// <summary>
    /// Raised when <see cref="Links"/> is updated.
    /// </summary>
    public event EventHandler<Link[]>? LinksUpdated;
    
    /// <summary>
    /// Raised when <see cref="ContactEmail"/> is updated.
    /// </summary>
    public event EventHandler<EmailConnection?>? ContactEmailUpdated;
    
    /// <summary>
    /// Raised when <see cref="IsPrivate"/> is updated.
    /// </summary>
    public event EventHandler<bool>? IsPrivateUpdated;

    /// <summary>
    /// Gets the owner of this publisher.
    /// </summary>
    public Task<IReadOnlyUser> GetOwnerAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the icon file for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task<IFile?> GetIconFileAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the projects for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Users who are registered to participate in this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IReadOnlyUser> GetUsersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the child publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IReadOnlyPublisher> GetChildPublishersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the parent publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IReadOnlyPublisher> GetParentPublishersAsync(CancellationToken cancellationToken);
}