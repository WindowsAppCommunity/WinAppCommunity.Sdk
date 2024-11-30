using System.Threading;
using System.Threading.Tasks;
using OwlCore.Storage;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk;

/// <summary>
/// Represents a user that can be modified. 
/// </summary>
public interface IModifiableUser : IReadOnlyUser, IModifiableEntity
{
    public Task UpdateNameAsync(string newName, CancellationToken cancellationToken);

    public Task UpdateForgetMeAsync(bool forget, CancellationToken cancellationToken);

    public Task AddConnectionAsync(IReadOnlyConnection value, CancellationToken cancellationToken);

    public Task RemoveConnectionAsync(IReadOnlyConnection value, CancellationToken cancellationToken);

    public Task AddLinkAsync(Link newLink, CancellationToken cancellationToken);

    public Task RemoveLinkAsync(Link linkToRemove, CancellationToken cancellationToken);

    public Task AddProjectAsync(IReadOnlyProject newProject, Role role, CancellationToken cancellationToken);

    public Task RemoveProjectAsync(IReadOnlyProject projectToRemove, CancellationToken cancellationToken);

    public Task AddPublisherAsync(IReadOnlyPublisher newPublisher, Role role, CancellationToken cancellationToken);

    public Task RemovePublisherAsync(IReadOnlyPublisher publisherToRemove, CancellationToken cancellationToken);
}
