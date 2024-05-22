using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.Extensions;
using OwlCore.Kubo;
using OwlCore.Nomad;
using OwlCore.Nomad.Extensions;
using OwlCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WinAppCommunity.Sdk.Models;
using WinAppCommunity.Sdk.Nomad;
using WinAppCommunity.Sdk.Nomad.Kubo;
using WinAppCommunity.Sdk.Nomad.Kubo.Extensions;
using WinAppCommunity.Sdk.Nomad.UpdateEvents;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Creates a new instance of <see cref="ModifiableUserAppModel"/>.
/// </summary>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>. </param>
public class ModifiableUserAppModel(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ModifiableUserNomadKuboEventStreamHandler(listeningEventStreamHandlers), IModifiableUser
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
    public async IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        foreach (var projectCid in Inner.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<Project>(projectCid, nocache: !UseCache, cancellationToken);
            Guard.IsNotNull(result);

            // assuming cid is ipns and won't change
            var ipnsId = projectCid;

            // If current node has write permissions
            if (existingKeys.FirstOrDefault(x => x.Id == ipnsId) is { } existingKey)
            {
                var appModel = new ModifiableProjectAppModel(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    Sources = Sources,
                    UseCache = UseCache,
                    Inner = result,
                    IpnsLifetime = IpnsLifetime,
                    LocalEventStreamKeyName = existingKey.Name,
                };

                await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow, (cid, ct) => NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client, UseCache, ct), cancellationToken).ToListAsync(cancellationToken);
                yield return appModel;
            }
            // If current node has no write permissions
            else
            {
                var appModel = new ReadOnlyProjectAppModel(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    Inner = result,
                    UseCache = UseCache,
                    Sources = Sources,
                };

                await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow, (cid, ct) => NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client, UseCache, ct), cancellationToken).ToListAsync(cancellationToken);
                yield return appModel;
            }
        }
    }

    /// <summary>
    /// Get the publishers for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<IReadOnlyPublisher> GetPublishersAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        foreach (var publisherCid in Inner.Publishers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<Publisher>(publisherCid, nocache: !UseCache, cancellationToken);
            Guard.IsNotNull(result);

            // assuming cid is ipns and won't change
            var ipnsId = publisherCid;

            // If current node has write permissions
            if (existingKeys.FirstOrDefault(x => x.Id == ipnsId) is { } existingKey)
            {
                var appModel = new ModifiablePublisherAppModel(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    Sources = Sources,
                    UseCache = UseCache,
                    Inner = result,
                    IpnsLifetime = IpnsLifetime,
                    LocalEventStreamKeyName = existingKey.Name,
                };

                await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow,
                    (cid, ct) =>
                        NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client, UseCache,
                            ct), cancellationToken).ToListAsync(cancellationToken);
                yield return appModel;
            }
            // If current node has no write permissions
            else
            {
                var appModel = new ReadOnlyPublisherAppModel(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    Inner = result,
                    UseCache = UseCache,
                    Sources = Sources,
                };

                await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow,
                    (cid, ct) =>
                        NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client, UseCache,
                            ct), cancellationToken).ToListAsync(cancellationToken);
                yield return appModel;
            }
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
