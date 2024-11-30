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
public interface IReadOnlyPublisher : IReadOnlyEntity
{
    /// <summary>
    /// A hex-encoded accent color for this publisher.
    /// </summary>
    public string? AccentColor { get; }

    /// <summary>
    /// A flag indicating whether this is a non-public project.
    /// </summary>
    public bool IsUnlisted { get; }

    /// <summary>
    /// Represents links to external profiles or resources added by the publisher.
    /// </summary>
    public Link[] Links { get; }

    /// <summary>
    /// A flag that indicates whether the entity has requested to be forgotten.
    /// </summary>
    public bool? ForgetMe { get; }

    /// <summary>
    /// Raised when <see cref="AccentColor"/> is updated.
    /// </summary>
    public event EventHandler<string?>? AccentColorUpdated;

    /// <summary>
    /// Raised when <see cref="IsUnlisted"/> is updated.
    /// </summary>
    public event EventHandler<bool>? IsUnlistedUpdated;

    /// <summary>
    /// Raised when <see cref="Links"/> is updated.
    /// </summary>
    public event EventHandler<Link[]>? LinksUpdated;

    /// <summary>
    /// Raised when <see cref="ForgetMe"/> is updated.
    /// </summary>
    public event EventHandler<bool?>? ForgetMeUpdated;

    /// <summary>
    /// Gets the users along with their roles for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<(IReadOnlyUser User, Role Role)> GetUsersWithRolesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the connections for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IReadOnlyConnection> GetConnectionsAsync(CancellationToken cancellationToken);

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
