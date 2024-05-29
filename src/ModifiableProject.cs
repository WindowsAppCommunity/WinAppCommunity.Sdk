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
/// Creates a new instance of <see cref="ModifiableProject"/>.
/// </summary>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>. </param>
public class ModifiableProject(
    ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>>
        listeningEventStreamHandlers)
    : ModifiableProjectNomadKuboEventStreamHandler(listeningEventStreamHandlers), IModifiableProject
{
    /// <inheritdoc />
    public string Name => Inner.Name;

    /// <inheritdoc />
    public string Description => Inner.Description;

    /// <inheritdoc />
    public string[] Features => Inner.Features;

    /// <inheritdoc />
    public string? AccentColor => Inner.AccentColor;

    /// <inheritdoc />
    public string Category => Inner.Category;

    /// <inheritdoc />
    public DateTime CreatedAt => Inner.CreatedAt;

    /// <inheritdoc />
    public Link[] Links => Inner.Links;

    /// <inheritdoc />
    public bool? ForgetMe => Inner.ForgetMe;

    /// <inheritdoc />
    public bool IsPrivate => Inner.IsPrivate;

    /// <inheritdoc />
    public ApplicationConnection[] Connections => Inner.Connections;

    /// <inheritdoc />
    public event EventHandler<string>? NameUpdated;

    /// <inheritdoc />
    public event EventHandler<string>? DescriptionUpdated;

    /// <inheritdoc />
    public event EventHandler<string[]>? FeaturesUpdated;

    /// <inheritdoc />
    public event EventHandler<string?>? AccentColorUpdated;

    /// <inheritdoc />
    public event EventHandler<string>? CategoryUpdated;

    /// <inheritdoc />
    public event EventHandler<DateTime>? CreatedAtUpdated;

    /// <inheritdoc />
    public event EventHandler<Link[]>? LinksUpdated;

    /// <inheritdoc />
    public event EventHandler<bool?>? ForgetMeUpdated;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPrivateUpdated;

    /// <inheritdoc />
    public event EventHandler<ApplicationConnection[]>? ConnectionsUpdated;

    /// <summary>
    /// Gets the image files for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<IFile> GetImageFilesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var imageCid in Inner.Images)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new IpfsFile(imageCid, $"{nameof(User)}.{Id}.png", Client);
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
    /// Gets the hero image file for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async Task<IFile?> GetHeroImageFileAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var heroImageCid = Inner.Icon;
        if (heroImageCid is null)
            return null;

        return new IpfsFile(heroImageCid, $"{nameof(User)}.{Id}.png", Client);
    }

    /// <summary>
    /// Gets the collaborators for this project.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<(IReadOnlyUser User, Role Role)> GetCollaboratorsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        foreach (var collaborator in Inner.Collaborators)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // assuming cid is ipns and won't change
            var ipnsId = collaborator.User;

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
                
                yield return (appModel, collaborator.Role);
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
                
                yield return (appModel, collaborator.Role);
            }
        }
    }

    /// <summary>
    /// Get the publisher for this project.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async Task<IReadOnlyPublisher> GetPublisherAsync(CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        cancellationToken.ThrowIfCancellationRequested();

        // assuming cid is ipns and won't change
        var ipnsId = Inner.Publisher;

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
                        KuboOptions.UseCache, ct), cancellationToken).ToListAsync(cancellationToken);
            
            _ = appModel.PublishRoamingAsync<ModifiablePublisher, PublisherUpdateEvent, Publisher>(cancellationToken);
            
            return appModel;
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
                        KuboOptions.UseCache, ct), cancellationToken).ToListAsync(cancellationToken);
            
            return appModel;
        }
    }

    /// <summary>
    /// Get the child publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<IReadOnlyProject> GetDependenciesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        foreach (var projectCid in Inner.Dependencies)
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

    /// <inheritdoc />
    public async Task UpdateNameAsync(string name, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectNameUpdateEvent(Id, name);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateDescriptionAsync(string description, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectDescriptionUpdateEvent(Id, description);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdatePublisherAsync(IReadOnlyPublisher publisher, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectPublisherUpdateEvent(Id, publisher.Id);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateIconAsync(IFile? iconFile, CancellationToken cancellationToken)
    {
        Cid? newCid = null;

        if (iconFile is not null)
        {
            using var stream = await iconFile.OpenReadAsync(cancellationToken);
            var fileSystemNode = await Client.FileSystem.AddAsync(stream, iconFile.Name, new AddFileOptions { Pin = KuboOptions.ShouldPin }, cancellationToken);
            newCid = fileSystemNode.Id;
        }

        var eventToUpdate = new ProjectIconUpdateEvent(Id, newCid);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateHeroImageAsync(IFile? heroImageFile, CancellationToken cancellationToken)
    {
        Cid? newCid = null;

        if (heroImageFile is not null)
        {
            using var stream = await heroImageFile.OpenReadAsync(cancellationToken);
            var fileSystemNode = await Client.FileSystem.AddAsync(stream, heroImageFile.Name, new AddFileOptions { Pin = KuboOptions.ShouldPin }, cancellationToken);
            newCid = fileSystemNode.Id;
        }
        
        var eventToUpdate = new ProjectHeroImageUpdateEvent(Id, newCid);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddFeatureAsync(string feature, CancellationToken cancellationToken)
    {
        var eventToAdd = new ProjectFeatureAddEvent(Id, feature);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveFeatureAsync(string feature, CancellationToken cancellationToken)
    {
        var eventToRemove = new ProjectFeatureRemoveEvent(Id, feature);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken)
    {
        using var stream = await imageFile.OpenReadAsync(cancellationToken);
        var fileSystemNode = await Client.FileSystem.AddAsync(stream, imageFile.Name, new AddFileOptions { Pin = KuboOptions.ShouldPin }, cancellationToken);
        var newCid = fileSystemNode.Id;
        
        var eventToAdd = new ProjectImageAddEvent(Id, newCid);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveImageAsync(IFile image, CancellationToken cancellationToken)
    {
        Cid cid = image.Id;
        var eventToRemove = new ProjectImageRemoveEvent(Id, cid);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddDependencyAsync(IReadOnlyProject dependency, CancellationToken cancellationToken)
    {
        var eventToAdd = new ProjectDependencyAddEvent(Id, dependency.Id);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveDependencyAsync(IReadOnlyProject dependency, CancellationToken cancellationToken)
    {
        var eventToRemove = new ProjectDependencyRemoveEvent(Id, dependency.Id);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddCollaboratorAsync(Collaborator collaborator, CancellationToken cancellationToken)
    {
        var eventToAdd = new ProjectCollaboratorAddEvent(Id, collaborator);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveCollaboratorAsync(Collaborator collaborator, CancellationToken cancellationToken)
    {
        var eventToRemove = new ProjectCollaboratorRemoveEvent(Id, collaborator);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddLinkAsync(Link link, CancellationToken cancellationToken)
    {
        var eventToAdd = new ProjectLinkAddEvent(Id, link);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveLinkAsync(Link link, CancellationToken cancellationToken)
    {
        var eventToRemove = new ProjectLinkRemoveEvent(Id, link);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddPublishedConnectionAsync(ApplicationConnection connection, CancellationToken cancellationToken)
    {
        var eventToAdd = new ProjectPublishedConnectionAddEvent(Id, connection);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemovePublishedConnectionAsync(ApplicationConnection connection,
        CancellationToken cancellationToken)
    {
        var eventToRemove = new ProjectPublishedConnectionRemoveEvent(Id, connection);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAccentColorAsync(string? accentColor, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectAccentColorUpdateEvent(Id, accentColor);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateCategoryAsync(string category, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectCategoryUpdateEvent(Id, category);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectForgetMeUpdateEvent(Id, forgetMe);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdatePrivateStateAsync(bool isPrivate, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectPrivacyUpdateEvent(Id, isPrivate);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }
}