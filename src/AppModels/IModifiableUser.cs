using Ipfs;
using System.Threading;
using System.Threading.Tasks;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk.AppModels;

public interface IModifiableUser : IReadOnlyUser
{
    public Task UpdateUserNameAsync(string newName, CancellationToken cancellationToken);

    public Task UpdateUserMarkdownAboutMeAsync(string newMarkdownAboutMe, CancellationToken cancellationToken);

    public Task UpdateUserIconAsync(Cid? newIcon, CancellationToken cancellationToken);

    public Task ForgetMeAsync(bool forget, CancellationToken cancellationToken);

    public Task AddConnectionAsync(ApplicationConnection newConnection, CancellationToken cancellationToken);

    public Task RemoveConnectionAsync(ApplicationConnection connectionToRemove, CancellationToken cancellationToken);

    public Task AddLinkAsync(Link newLink, CancellationToken cancellationToken);

    public Task RemoveLinkAsync(Link linkToRemove, CancellationToken cancellationToken);

    public Task AddProjectAsync(Cid newProject, CancellationToken cancellationToken);

    public Task RemoveProjectAsync(Cid projectToRemove, CancellationToken cancellationToken);

    public Task AddPublisherAsync(Cid newPublisher, CancellationToken cancellationToken);

    public Task RemovePublisherAsync(Cid publisherToRemove, CancellationToken cancellationToken);
}