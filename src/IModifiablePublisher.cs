using System.Threading;
using System.Threading.Tasks;
using OwlCore.Storage;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk;

/// <summary>
/// Represents a content publisher that can be modified.
/// </summary>
public interface IModifiablePublisher : IReadOnlyPublisher, IModifiableEntity
{
    /// <summary>
    /// Updates the name of this publisher.
    /// </summary>
    public Task UpdateNameAsync(string newName, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the description for this publisher.
    /// </summary>
    public Task UpdateDescriptionAsync(string newDescription, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the accent color for this publisher.
    /// </summary>
    public Task UpdateAccentColorAsync(string? newAccentColor, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a link to this publisher.
    /// </summary>
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the link from this publisher.
    /// </summary>
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a user to this publisher along with their role.
    /// </summary>
    public Task AddUserAsync(IReadOnlyUser user, Role role, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the user from this publisher.
    /// </summary>
    public Task RemoveUserAsync(IReadOnlyUser user, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a child publisher to this publisher.
    /// </summary>
    public Task AddChildPublisherAsync(IReadOnlyPublisher childPublisher, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a child publisher from this publisher.
    /// </summary>
    public Task RemoveChildPublisherAsync(IReadOnlyPublisher childPublisher, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a parent publisher to this publisher.
    /// </summary>
    public Task AddParentPublisherAsync(IReadOnlyPublisher parentPublisher, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a parent publisher from this publisher.
    /// </summary>
    public Task RemoveParentPublisherAsync(IReadOnlyPublisher parentPublisher, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a connection to this publisher.
    /// </summary>
    public Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a connection from this publisher.
    /// </summary>
    public Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the private flag for this publisher.
    /// </summary>
    public Task UpdatePrivateFlagAsync(bool isPrivate, CancellationToken cancellationToken);
}
