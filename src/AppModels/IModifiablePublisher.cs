using Ipfs;
using System.Threading;
using System.Threading.Tasks;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk.AppModels;

public interface IModifiablePublisher : IReadOnlyPublisher
{
    public Task UpdateNameAsync(string newName, CancellationToken cancellationToken);

    public Task UpdateDescriptionAsync(string newDescription, CancellationToken cancellationToken);

    public Task UpdateIconAsync(Cid? newIcon, CancellationToken cancellationToken);

    public Task UpdateAccentColorAsync(string? newAccentColor, CancellationToken cancellationToken);

    public Task UpdateContactEmailAsync(EmailConnection? newContactEmail, CancellationToken cancellationToken);

    public Task AddLinkAsync(Link link, CancellationToken cancellationToken);

    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken);

    public Task AddProjectAsync(Cid publisher, CancellationToken cancellationToken);

    public Task RemoveProjectAsync(Cid publisher, CancellationToken cancellationToken);

    public Task AddChildPublisherAsync(Cid childPublisher, CancellationToken cancellationToken);

    public Task RemoveChildPublisherAsync(Cid childPublisher, CancellationToken cancellationToken);

    public Task AddParentPublisherAsync(Cid parentPublisher, CancellationToken cancellationToken);

    public Task RemoveParentPublisherAsync(Cid parentPublisher, CancellationToken cancellationToken);

    public Task UpdatePrivateFlagAsync(bool isPrivate, CancellationToken cancellationToken);
}