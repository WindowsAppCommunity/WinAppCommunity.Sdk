using Ipfs;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Storage;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Represents a user that can be modified. 
/// </summary>
public interface IModifiableUser : IReadOnlyUser
{
    public Task UpdateNameAsync(string newName, CancellationToken cancellationToken);

    public Task UpdateMarkdownAboutMeAsync(string newMarkdownAboutMe, CancellationToken cancellationToken);

    public Task UpdateIconAsync(IFile? newIconFile, CancellationToken cancellationToken);

    public Task UpdateForgetMeAsync(bool forget, CancellationToken cancellationToken);

    public Task AddConnectionAsync(ApplicationConnection newConnection, CancellationToken cancellationToken);

    public Task RemoveConnectionAsync(ApplicationConnection connectionToRemove, CancellationToken cancellationToken);

    public Task AddLinkAsync(Link newLink, CancellationToken cancellationToken);

    public Task RemoveLinkAsync(Link linkToRemove, CancellationToken cancellationToken);

    public Task AddProjectAsync(IReadOnlyProject newProject, CancellationToken cancellationToken);

    public Task RemoveProjectAsync(IReadOnlyProject projectToRemove, CancellationToken cancellationToken);

    public Task AddPublisherAsync(IReadOnlyPublisher newPublisher, CancellationToken cancellationToken);

    public Task RemovePublisherAsync(IReadOnlyPublisher publisherToRemove, CancellationToken cancellationToken);
}