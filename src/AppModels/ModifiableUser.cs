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
using Ipfs.CoreApi;
using WinAppCommunity.Sdk.Models;
using WinAppCommunity.Sdk.Nomad;
using WinAppCommunity.Sdk.Nomad.Kubo;
using WinAppCommunity.Sdk.Nomad.Kubo.Extensions;
using WinAppCommunity.Sdk.Nomad.UpdateEvents;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Creates a new instance of <see cref="ModifiableUser"/>.
/// </summary>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>. </param>
public class ModifiableUser(
    ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>>
        listeningEventStreamHandlers)
    : ModifiableUserNomadKuboEventStreamHandler(listeningEventStreamHandlers), IModifiableUser
{
    /// <inheritdoc />
    public string Name => Inner.Name;

    /// <inheritdoc />
    public string MarkdownAboutMe => Inner.MarkdownAboutMe;

    /// <inheritdoc />
    public ApplicationConnection[] Connections => Inner.Connections;

    /// <inheritdoc />
    public Link[] Links => Inner.Links;

    /// <inheritdoc />
    public bool? ForgetMe => Inner.ForgetMe;

    /// <inheritdoc />
    public event EventHandler<string>? NameUpdated;

    /// <inheritdoc />
    public event EventHandler<string>? MarkdownAboutMeUpdated;

    /// <inheritdoc />
    public event EventHandler<ApplicationConnection[]>? ConnectionsUpdated;

    /// <inheritdoc />
    public event EventHandler<Link[]>? LinksUpdated;

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
    public async IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        foreach (var projectCid in Inner.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // assuming cid is ipns and won't change
            var ipnsId = projectCid;

            // If current node has write permissions
            if (existingKeys.FirstOrDefault(x => x.Id == ipnsId) is { } existingKey)
            {
                var appModel = new ModifiableProject(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    Sources = Sources,
                    KuboOptions = KuboOptions,
                    LocalEventStreamKeyName = LocalEventStreamKeyName,
                    RoamingKeyName = existingKey.Name,
                };

                await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow,
                    (cid, ct) =>
                        NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client,
                            KuboOptions.UseCache, ct), cancellationToken).ToListAsync(cancellationToken);
                _ = appModel.PublishRoamingAsync<ModifiableProject, ProjectUpdateEvent, Project>(
                    cancellationToken);

                yield return appModel;
            }
            // If current node has no write permissions
            else
            {
                var appModel = new ReadOnlyProject(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    KuboOptions = KuboOptions,
                    Sources = Sources,
                    LocalEventStreamKeyName = LocalEventStreamKeyName,
                };

                await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow,
                    (cid, ct) =>
                        NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client,
                            KuboOptions.UseCache, ct), cancellationToken).ToListAsync(cancellationToken);
                yield return appModel;
            }
        }
    }

    /// <summary>
    /// Get the publishers for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<IReadOnlyPublisher> GetPublishersAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        foreach (var publisherCid in Inner.Publishers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // assuming cid is ipns and won't change
            var ipnsId = publisherCid;

            // If current node has write permissions
            if (existingKeys.FirstOrDefault(x => x.Id == ipnsId) is { } existingKey)
            {
                var appModel = new ModifiablePublisher(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    Sources = Sources,
                    KuboOptions = KuboOptions,
                    LocalEventStreamKeyName = LocalEventStreamKeyName,
                    RoamingKeyName = existingKey.Name,
                };

                await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow,
                    (cid, ct) =>
                        NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client,
                            KuboOptions.UseCache,
                            ct), cancellationToken).ToListAsync(cancellationToken);

                _ = appModel.PublishRoamingAsync<ModifiablePublisher, PublisherUpdateEvent, Publisher>(
                    cancellationToken);

                yield return appModel;
            }
            // If current node has no write permissions
            else
            {
                var appModel = new ReadOnlyPublisher(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    Sources = Sources,
                    KuboOptions = KuboOptions,
                    LocalEventStreamKeyName = LocalEventStreamKeyName,
                };

                await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow,
                    (cid, ct) =>
                        NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client,
                            KuboOptions.UseCache,
                            ct), cancellationToken).ToListAsync(cancellationToken);
                
                yield return appModel;
            }
        }
    }

    public async Task UpdateNameAsync(string newName, CancellationToken cancellationToken)
    {
        var updateEvent = new UserNameUpdateEvent(Id, newName);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task UpdateMarkdownAboutMeAsync(string newMarkdownAboutMe, CancellationToken cancellationToken)
    {
        var updateEvent = new UserMarkdownAboutMeUpdateEvent(Id, newMarkdownAboutMe);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task UpdateIconAsync(IFile? iconFile, CancellationToken cancellationToken)
    {
        Cid? newCid = null;

        if (iconFile is not null)
        {
            using var stream = await iconFile.OpenReadAsync(cancellationToken);
            var fileSystemNode = await Client.FileSystem.AddAsync(stream, iconFile.Name,
                new AddFileOptions { Pin = KuboOptions.ShouldPin }, cancellationToken);
            newCid = fileSystemNode.Id;
        }

        var updateEvent = new UserIconUpdateEvent(Id, newCid);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task UpdateForgetMeAsync(bool forget, CancellationToken cancellationToken)
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

    public async Task RemoveConnectionAsync(ApplicationConnection connectionToRemove,
        CancellationToken cancellationToken)
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

    public async Task AddProjectAsync(IReadOnlyProject newProject, CancellationToken cancellationToken)
    {
        var updateEvent = new UserProjectAddEvent(Id, newProject.Id);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task RemoveProjectAsync(IReadOnlyProject projectToRemove, CancellationToken cancellationToken)
    {
        var updateEvent = new UserProjectRemoveEvent(Id, projectToRemove.Id);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task AddPublisherAsync(IReadOnlyPublisher newPublisher, CancellationToken cancellationToken)
    {
        var updateEvent = new UserPublisherAddEvent(Id, newPublisher.Id);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task RemovePublisherAsync(IReadOnlyPublisher publisherToRemove, CancellationToken cancellationToken)
    {
        var updateEvent = new UserPublisherRemoveEvent(Id, publisherToRemove.Id);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }
}