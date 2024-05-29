using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.Extensions;
using OwlCore.Kubo;
using OwlCore.Nomad;
using OwlCore.Nomad.Extensions;
using OwlCore.Storage;
using WinAppCommunity.Sdk.Models;
using WinAppCommunity.Sdk.Nomad;
using WinAppCommunity.Sdk.Nomad.Kubo;
using WinAppCommunity.Sdk.Nomad.Kubo.Extensions;
using WinAppCommunity.Sdk.Nomad.UpdateEvents;

namespace WinAppCommunity.Sdk;

/// <summary>
/// Creates a new instance of <see cref="ModifiablePublisher"/>.
/// </summary>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>. </param>
public class ModifiablePublisher(
    ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>>
        listeningEventStreamHandlers)
    : ModifiablePublisherNomadKuboEventStreamHandler(listeningEventStreamHandlers), IModifiablePublisher
{
    /// <inheritdoc />
    public string Name => Inner.Name;

    /// <inheritdoc />
    public string Description => Inner.Description;

    /// <inheritdoc />
    public string? AccentColor => Inner.AccentColor;

    /// <inheritdoc />
    public Link[] Links => Inner.Links;

    /// <inheritdoc />
    public EmailConnection? ContactEmail => Inner.ContactEmail;

    /// <inheritdoc />
    public bool IsPrivate => Inner.IsPrivate;

    /// <inheritdoc />
    public event EventHandler<string>? NameUpdated;

    /// <inheritdoc />
    public event EventHandler<string>? DescriptionUpdated;

    /// <inheritdoc />
    public event EventHandler<string?>? AccentColorUpdated;

    /// <inheritdoc />
    public event EventHandler<Link[]>? LinksUpdated;

    /// <inheritdoc />
    public event EventHandler<EmailConnection?>? ContactEmailUpdated;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPrivateUpdated;

    /// <inheritdoc />
    public async Task<IReadOnlyUser> GetOwnerAsync(CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();
        cancellationToken.ThrowIfCancellationRequested();

        // assuming cid is ipns and won't change
        var ipnsId = Inner.Owner;

        // If current node has write permissions
        if (existingKeys.FirstOrDefault(x => x.Id == ipnsId) is { } existingKey)
        {
            var appModel = new ModifiableUser(ListeningEventStreamHandlers)
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

            _ = appModel.PublishRoamingAsync<ModifiableUser, UserUpdateEvent, User>(cancellationToken);

            return appModel;
        }
        // If current node has no write permissions
        else
        {
            var appModel = new ReadOnlyUser(ListeningEventStreamHandlers)
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
            
            return appModel;
        }
    }

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
    /// Get the projects for this publisher.
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

                _ = appModel.PublishRoamingAsync<ModifiableProject, ProjectUpdateEvent, Project>(cancellationToken);
                
                yield return appModel;
            }
            // If current node has no write permissions
            else
            {
                var appModel = new ReadOnlyProject(ListeningEventStreamHandlers)
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
                            KuboOptions.UseCache, ct), cancellationToken).ToListAsync(cancellationToken);
                
                yield return appModel;
            }
        }
    }

    /// <summary>
    /// Get the projects for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<IReadOnlyUser> GetUsersAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        foreach (var projectCid in Inner.Users)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // assuming cid is ipns and won't change
            var ipnsId = projectCid;

            // If current node has write permissions
            if (existingKeys.FirstOrDefault(x => x.Id == ipnsId) is { } existingKey)
            {
                var appModel = new ModifiableUser(ListeningEventStreamHandlers)
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

                _ = appModel.PublishRoamingAsync<ModifiableUser, UserUpdateEvent, User>(cancellationToken);

                yield return appModel;
            }
            // If current node has no write permissions
            else
            {
                var appModel = new ReadOnlyUser(ListeningEventStreamHandlers)
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
    /// Get the publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<IReadOnlyPublisher> GetChildPublishersAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        foreach (var publisherCid in Inner.ChildPublishers)
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

                _ = appModel.PublishRoamingAsync<ModifiablePublisher, PublisherUpdateEvent, Publisher>(cancellationToken);
                
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

    /// <summary>
    /// Get the publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<IReadOnlyPublisher> GetParentPublishersAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        foreach (var publisherCid in Inner.ParentPublishers)
        {
            cancellationToken.ThrowIfCancellationRequested();
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

                _ = appModel.PublishRoamingAsync<ModifiablePublisher, PublisherUpdateEvent, Publisher>(cancellationToken);
                
                yield return appModel;
            }
            // If current node has no write permissions
            else
            {
                var appModel = new ReadOnlyPublisher(ListeningEventStreamHandlers)
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
                            KuboOptions.UseCache,
                            ct), cancellationToken).ToListAsync(cancellationToken);
                yield return appModel;
            }
        }
    }

    /// <inheritdoc />
    public async Task UpdateNameAsync(string newName, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherNameUpdateEvent(Id, newName);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateDescriptionAsync(string newDescription, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherDescriptionUpdateEvent(Id, newDescription);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateOwnerAsync(IReadOnlyUser user, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherOwnerUpdateEvent(Id, user.Id);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    /// <inheritdoc />
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

        var updateEvent = new PublisherIconUpdateEvent(Id, newCid);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAccentColorAsync(string? newAccentColor, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherAccentColorUpdateEvent(Id, newAccentColor);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateContactEmailAsync(EmailConnection? newContactEmail, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherContactEmailUpdateEvent(Id, newContactEmail);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddLinkAsync(Link link, CancellationToken cancellationToken)
    {
        var addEvent = new PublisherLinkAddEvent(Id, link);
        await ApplyEntryUpdateAsync(addEvent, cancellationToken);
        await AppendNewEntryAsync(addEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveLinkAsync(Link link, CancellationToken cancellationToken)
    {
        var removeEvent = new PublisherLinkRemoveEvent(Id, link);
        await ApplyEntryUpdateAsync(removeEvent, cancellationToken);
        await AppendNewEntryAsync(removeEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddProjectAsync(IReadOnlyProject project, CancellationToken cancellationToken)
    {
        var addEvent = new PublisherProjectAddEvent(Id, project.Id);
        await ApplyEntryUpdateAsync(addEvent, cancellationToken);
        await AppendNewEntryAsync(addEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveProjectAsync(IReadOnlyProject project, CancellationToken cancellationToken)
    {
        var removeEvent = new PublisherProjectRemoveEvent(Id, project.Id);
        await ApplyEntryUpdateAsync(removeEvent, cancellationToken);
        await AppendNewEntryAsync(removeEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddUserAsync(IReadOnlyUser user, CancellationToken cancellationToken)
    {
        var removeEvent = new PublisherUserAddEvent(Id, user.Id);
        await ApplyEntryUpdateAsync(removeEvent, cancellationToken);
        await AppendNewEntryAsync(removeEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveUserAsync(IReadOnlyUser user, CancellationToken cancellationToken)
    {
        var removeEvent = new PublisherUserRemoveEvent(Id, user.Id);
        await ApplyEntryUpdateAsync(removeEvent, cancellationToken);
        await AppendNewEntryAsync(removeEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddChildPublisherAsync(IReadOnlyPublisher childPublisher, CancellationToken cancellationToken)
    {
        var addEvent = new PublisherChildPublisherAddEvent(Id, childPublisher.Id);
        await ApplyEntryUpdateAsync(addEvent, cancellationToken);
        await AppendNewEntryAsync(addEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveChildPublisherAsync(IReadOnlyPublisher childPublisher, CancellationToken cancellationToken)
    {
        var removeEvent = new PublisherChildPublisherRemoveEvent(Id, childPublisher.Id);
        await ApplyEntryUpdateAsync(removeEvent, cancellationToken);
        await AppendNewEntryAsync(removeEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddParentPublisherAsync(IReadOnlyPublisher parentPublisher, CancellationToken cancellationToken)
    {
        var addEvent = new PublisherParentPublisherAddEvent(Id, parentPublisher.Id);
        await ApplyEntryUpdateAsync(addEvent, cancellationToken);
        await AppendNewEntryAsync(addEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveParentPublisherAsync(IReadOnlyPublisher parentPublisher,
        CancellationToken cancellationToken)
    {
        var removeEvent = new PublisherParentPublisherRemoveEvent(Id, parentPublisher.Id);
        await ApplyEntryUpdateAsync(removeEvent, cancellationToken);
        await AppendNewEntryAsync(removeEvent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdatePrivateFlagAsync(bool isPrivate, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherPrivateFlagUpdateEvent(Id, isPrivate);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }
}