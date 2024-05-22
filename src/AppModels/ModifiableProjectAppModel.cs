using CommunityToolkit.Diagnostics;
using Google.Protobuf.WellKnownTypes;
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
/// Creates a new instance of <see cref="ModifiableProjectAppModel"/>.
/// </summary>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>. </param>
public class ModifiableProjectAppModel(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ModifiableProjectNomadKuboEventStreamHandler(listeningEventStreamHandlers), IModifiableProject
{
    /// <summary>
    /// Gets the image files for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<IFile> GetImageFilesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<(IReadOnlyUser User, Role Role)> GetCollaboratorsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        foreach (var collaborator in Inner.Collaborators)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<User>(collaborator.User, nocache: !UseCache, cancellationToken);
            Guard.IsNotNull(result);

            // assuming cid is ipns and won't change
            var ipnsId = Inner.Publisher;

            ReadOnlyUserNomadKuboEventStreamHandler userAppModel = existingKeys.FirstOrDefault(x => x.Id == ipnsId) is { } value
                // If current node has write permissions
                ? new ModifiableUserAppModel(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    Sources = Sources,
                    UseCache = UseCache,
                    Inner = result,
                    IpnsLifetime = IpnsLifetime,
                    LocalEventStreamKeyName = value.Name,
                }
                :
                // If current node has no write permissions
                new ReadOnlyUserAppModel(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    Inner = result,
                    UseCache = UseCache,
                    Sources = Sources,
                };

            await userAppModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow, (cid, ct) => NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client, UseCache, ct), cancellationToken).ToListAsync(cancellationToken);
            yield return ((IReadOnlyUser)userAppModel, collaborator.Role);
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
        var (result, _) = await Client.ResolveDagCidAsync<Publisher>(Inner.Publisher, nocache: !UseCache, cancellationToken);
        Guard.IsNotNull(result);

        // assuming cid is ipns and won't change
        var ipnsId = Inner.Publisher;

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

            await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow, (cid, ct) => NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client, UseCache, ct), cancellationToken).ToListAsync(cancellationToken);
            return appModel;
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

            await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow, (cid, ct) => NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client, UseCache, ct), cancellationToken).ToListAsync(cancellationToken);
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
            var (result, _) = await Client.ResolveDagCidAsync<Project>(projectCid, nocache: !UseCache, cancellationToken);
            Guard.IsNotNull(result);
            
            // assuming cid is ipns and won't change
            var ipnsId = Inner.Publisher;

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

    public async Task UpdateProjectNameAsync(string name, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectNameUpdateEvent(Id, name);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    public async Task UpdateProjectDescriptionAsync(string description, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectDescriptionUpdateEvent(Id, description);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    public async Task UpdateProjectIconAsync(Cid? icon, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectIconUpdateEvent(Id, icon);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    public async Task UpdateProjectHeroImageAsync(Cid? heroImage, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectHeroImageUpdateEvent(Id, heroImage);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    public async Task AddProjectFeatureAsync(string feature, CancellationToken cancellationToken)
    {
        var eventToAdd = new ProjectFeatureAddEvent(Id, feature);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    public async Task RemoveProjectFeatureAsync(string feature, CancellationToken cancellationToken)
    {
        var eventToRemove = new ProjectFeatureRemoveEvent(Id, feature);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    public async Task AddProjectImageAsync(Cid image, CancellationToken cancellationToken)
    {
        var eventToAdd = new ProjectImageAddEvent(Id, image);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    public async Task RemoveProjectImageAsync(Cid image, CancellationToken cancellationToken)
    {
        var eventToRemove = new ProjectImageRemoveEvent(Id, image);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    public async Task AddProjectDependencyAsync(Cid dependency, CancellationToken cancellationToken)
    {
        var eventToAdd = new ProjectDependencyAddEvent(Id, dependency);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    public async Task RemoveProjectDependencyAsync(Cid dependency, CancellationToken cancellationToken)
    {
        var eventToRemove = new ProjectDependencyRemoveEvent(Id, dependency);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    public async Task AddProjectCollaboratorAsync(Collaborator collaborator, CancellationToken cancellationToken)
    {
        var eventToAdd = new ProjectCollaboratorAddEvent(Id, collaborator);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    public async Task RemoveProjectCollaboratorAsync(Collaborator collaborator, CancellationToken cancellationToken)
    {
        var eventToRemove = new ProjectCollaboratorRemoveEvent(Id, collaborator);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    public async Task AddProjectLinkAsync(Link link, CancellationToken cancellationToken)
    {
        var eventToAdd = new ProjectLinkAddEvent(Id, link);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    public async Task RemoveProjectLinkAsync(Link link, CancellationToken cancellationToken)
    {
        var eventToRemove = new ProjectLinkRemoveEvent(Id, link);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    public async Task AddProjectPublishedConnectionAsync(ApplicationConnection connection, CancellationToken cancellationToken)
    {
        var eventToAdd = new ProjectPublishedConnectionAddEvent(Id, connection);
        await ApplyEntryUpdateAsync(eventToAdd, cancellationToken);
        await AppendNewEntryAsync(eventToAdd, cancellationToken);
    }

    public async Task RemoveProjectPublishedConnectionAsync(ApplicationConnection connection, CancellationToken cancellationToken)
    {
        var eventToRemove = new ProjectPublishedConnectionRemoveEvent(Id, connection);
        await ApplyEntryUpdateAsync(eventToRemove, cancellationToken);
        await AppendNewEntryAsync(eventToRemove, cancellationToken);
    }

    public async Task UpdateProjectAccentColorAsync(string? accentColor, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectAccentColorUpdateEvent(Id, accentColor);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    public async Task UpdateProjectCategoryAsync(string category, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectCategoryUpdateEvent(Id, category);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    public async Task UpdateProjectForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectForgetMeUpdateEvent(Id, forgetMe);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

    public async Task UpdateProjectPrivacyStatusAsync(bool isPrivate, CancellationToken cancellationToken)
    {
        var eventToUpdate = new ProjectPrivacyUpdateEvent(Id, isPrivate);
        await ApplyEntryUpdateAsync(eventToUpdate, cancellationToken);
        await AppendNewEntryAsync(eventToUpdate, cancellationToken);
    }

}