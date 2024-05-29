using Ipfs;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Storage;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Represents a project that can be modified.
/// </summary>
public interface IModifiableProject : IReadOnlyProject
{
    /// <summary>
    /// Updates the name of this publisher.
    /// </summary>
    public Task UpdateNameAsync(string name, CancellationToken cancellationToken);
    
    /// <summary>
    /// Updates the publisher for this project.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task UpdatePublisherAsync(IReadOnlyPublisher publisher, CancellationToken cancellationToken);
    
    /// <summary>
    /// Updates the description of this publisher.
    /// </summary>
    public Task UpdateDescriptionAsync(string description, CancellationToken cancellationToken);

    public Task UpdateIconAsync(IFile? iconFile, CancellationToken cancellationToken);

    public Task UpdateHeroImageAsync(IFile? heroImageFile, CancellationToken cancellationToken);

    public Task AddFeatureAsync(string feature, CancellationToken cancellationToken);

    public Task RemoveFeatureAsync(string feature, CancellationToken cancellationToken);

    public Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken);

    public Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken);

    public Task AddDependencyAsync(IReadOnlyProject projectDependency, CancellationToken cancellationToken);

    public Task RemoveDependencyAsync(IReadOnlyProject projectDependency, CancellationToken cancellationToken);

    public Task AddCollaboratorAsync(Collaborator collaborator, CancellationToken cancellationToken);

    public Task RemoveCollaboratorAsync(Collaborator collaborator, CancellationToken cancellationToken);

    public Task AddLinkAsync(Link link, CancellationToken cancellationToken);

    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken);

    public Task AddPublishedConnectionAsync(ApplicationConnection connection, CancellationToken cancellationToken);

    public Task RemovePublishedConnectionAsync(ApplicationConnection connection, CancellationToken cancellationToken);

    public Task UpdateAccentColorAsync(string? accentColor, CancellationToken cancellationToken);

    public Task UpdateCategoryAsync(string category, CancellationToken cancellationToken);

    public Task UpdateForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken);

    public Task UpdatePrivateStateAsync(bool isPrivate, CancellationToken cancellationToken);
}