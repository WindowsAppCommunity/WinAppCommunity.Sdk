using Ipfs;
using System.Threading;
using System.Threading.Tasks;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Represents a project that can be modified.
/// </summary>
public interface IModifiableProject : IReadOnlyProject
{
    public Task UpdateProjectNameAsync(string name, CancellationToken cancellationToken);

    public Task UpdateProjectDescriptionAsync(string description, CancellationToken cancellationToken);

    public Task UpdateProjectIconAsync(Cid? icon, CancellationToken cancellationToken);

    public Task UpdateProjectHeroImageAsync(Cid? heroImage, CancellationToken cancellationToken);

    public Task AddProjectFeatureAsync(string feature, CancellationToken cancellationToken);

    public Task RemoveProjectFeatureAsync(string feature, CancellationToken cancellationToken);

    public Task AddProjectImageAsync(Cid image, CancellationToken cancellationToken);

    public Task RemoveProjectImageAsync(Cid image, CancellationToken cancellationToken);

    public Task AddProjectDependencyAsync(Cid dependency, CancellationToken cancellationToken);

    public Task RemoveProjectDependencyAsync(Cid dependency, CancellationToken cancellationToken);

    public Task AddProjectCollaboratorAsync(Collaborator collaborator, CancellationToken cancellationToken);

    public Task RemoveProjectCollaboratorAsync(Collaborator collaborator, CancellationToken cancellationToken);

    public Task AddProjectLinkAsync(Link link, CancellationToken cancellationToken);

    public Task RemoveProjectLinkAsync(Link link, CancellationToken cancellationToken);

    public Task AddProjectPublishedConnectionAsync(ApplicationConnection connection, CancellationToken cancellationToken);

    public Task RemoveProjectPublishedConnectionAsync(ApplicationConnection connection, CancellationToken cancellationToken);

    public Task UpdateProjectAccentColorAsync(string? accentColor, CancellationToken cancellationToken);

    public Task UpdateProjectCategoryAsync(string category, CancellationToken cancellationToken);

    public Task UpdateProjectForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken);

    public Task UpdateProjectPrivacyStatusAsync(bool isPrivate, CancellationToken cancellationToken);
}