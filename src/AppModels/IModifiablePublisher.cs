using Ipfs;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Storage;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Represents a content publisher that can be modified.
/// </summary>
public interface IModifiablePublisher : IReadOnlyPublisher
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
    /// Updates the owner of this publisher.
    /// </summary>
    public Task UpdateOwnerAsync(IReadOnlyUser user, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the icon for this publisher.
    /// </summary>
    public Task UpdateIconAsync(IFile? newIconFile, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the accent color for this publisher.
    /// </summary>
    public Task UpdateAccentColorAsync(string? newAccentColor, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the contact email for this publisher.
    /// </summary>
    public Task UpdateContactEmailAsync(EmailConnection? newContactEmail, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a link to this publisher.
    /// </summary>
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the link from this publisher.
    /// </summary>
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a project to this publisher.
    /// </summary>
    public Task AddProjectAsync(IReadOnlyProject project, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the project from this publisher.
    /// </summary>
    public Task RemoveProjectAsync(IReadOnlyProject project, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a user to this publisher.
    /// </summary>
    public Task AddUserAsync(IReadOnlyUser user, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the user from this publisher.
    /// </summary>
    public Task RemoveUserAsync(IReadOnlyUser project, CancellationToken cancellationToken);

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
    /// Updates the private flag for this publisher.
    /// </summary>
    public Task UpdatePrivateFlagAsync(bool isPrivate, CancellationToken cancellationToken);
}