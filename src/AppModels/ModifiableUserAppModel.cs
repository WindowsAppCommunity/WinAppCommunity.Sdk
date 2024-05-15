using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.ComponentModel.Nomad;
using OwlCore.Kubo;
using OwlCore.Storage;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WinAppCommunity.Sdk.Models;
using WinAppCommunity.Sdk.Nomad;
using WinAppCommunity.Sdk.Nomad.Kubo;
using WinAppCommunity.Sdk.Nomad.UpdateEvents;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Creates a new instance of <see cref="ModifiableUserAppModel"/>.
/// </summary>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{T}.TryAdvanceEventStreamAsync"/>. </param>
public class ModifiableUserAppModel(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ModifiableUserNomadKuboEventStreamHandler(listeningEventStreamHandlers)
{
    /// <summary>
    /// Gets the icon file for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async Task<IFile?> GetIconFileAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var iconCid = Inner.Icon;
        if (iconCid is null)
            return null;

        return new IpfsFile(iconCid, $"{nameof(User)}.{Id}.png", Client);
    }

    /// <summary>
    /// Get the projects for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<ModifiableProjectAppModel> GetProjectsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var projectCid in Inner.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<Project>(projectCid, nocache: !UseCache, cancellationToken);
            Guard.IsNotNull(result);

            yield return new ModifiableProjectAppModel(ListeningEventStreamHandlers)
            {
                Client = Client,
                LocalEventStreamKeyName = LocalEventStreamKeyName,
                Id = projectCid, // assuming project cid is ipns and won't change
                IpnsLifetime = IpnsLifetime,
                Sources = Sources,
                UseCache = UseCache,
                ShouldPin = ShouldPin,
                Inner = result,
            };
        }
    }

    /// <summary>
    /// Get the publishers for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<ModifiablePublisherAppModel> GetPublishersAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var publisherCid in Inner.Publishers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<Publisher>(publisherCid, nocache: !UseCache, cancellationToken);
            Guard.IsNotNull(result);

            yield return new ModifiablePublisherAppModel(ListeningEventStreamHandlers)
            {
                Client = Client,
                LocalEventStreamKeyName = LocalEventStreamKeyName,
                Id = publisherCid, // assuming publisher cid is ipns and won't change
                IpnsLifetime = IpnsLifetime,
                Sources = Sources,
                UseCache = UseCache,
                ShouldPin = ShouldPin,
                Inner = result,
            };
        }
    }

    public async Task UpdateUserNameAsync(string newName, CancellationToken cancellationToken)
    {
        var updateEvent = new UserNameUpdateEvent(Id, newName);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task UpdateUserMarkdownAboutMeAsync(string newMarkdownAboutMe, CancellationToken cancellationToken)
    {
        var updateEvent = new UserMarkdownAboutMeUpdateEvent(Id, newMarkdownAboutMe);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task UpdateUserIconAsync(Cid? newIcon, CancellationToken cancellationToken)
    {
        var updateEvent = new UserIconUpdateEvent(Id, newIcon);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task ForgetMeAsync(bool forget, CancellationToken cancellationToken)
    {
        var updateEvent = new UserForgetMeUpdateEvent(Id, forget);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task AddConnectionAsync(ApplicationConnection newConnection, CancellationToken cancellationToken)
    {
        var updateEvent = new UserConnectionAddEvent(Id, newConnection);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task RemoveConnectionAsync(ApplicationConnection connectionToRemove, CancellationToken cancellationToken)
    {
        var updateEvent = new UserConnectionRemoveEvent(Id, connectionToRemove);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task AddLinkAsync(Link newLink, CancellationToken cancellationToken)
    {
        var updateEvent = new UserLinkAddEvent(Id, newLink);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task RemoveLinkAsync(Link linkToRemove, CancellationToken cancellationToken)
    {
        var updateEvent = new UserLinkRemoveEvent(Id, linkToRemove);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task AddProjectAsync(Cid newProject, CancellationToken cancellationToken)
    {
        var updateEvent = new UserProjectAddEvent(Id, newProject);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task RemoveProjectAsync(Cid projectToRemove, CancellationToken cancellationToken)
    {
        var updateEvent = new UserProjectRemoveEvent(Id, projectToRemove);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task AddPublisherAsync(Cid newPublisher, CancellationToken cancellationToken)
    {
        var updateEvent = new UserPublisherAddEvent(Id, newPublisher);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task RemovePublisherAsync(Cid publisherToRemove, CancellationToken cancellationToken)
    {
        var updateEvent = new UserPublisherRemoveEvent(Id, publisherToRemove);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }
}
