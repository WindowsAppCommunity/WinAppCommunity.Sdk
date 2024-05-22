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

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Creates a new instance of <see cref="ReadOnlyProjectAppModel"/>.
/// </summary>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>. </param>
public class ReadOnlyProjectAppModel(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ReadOnlyProjectNomadKuboEventStreamHandler(listeningEventStreamHandlers), IReadOnlyProject
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
    /// Gets the hero image file for this project.
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

    private async Task<KuboNomadEventStreamEntry> ContentPointerToStreamEntryAsync(Cid cid, CancellationToken token)
    {
        var (streamEntry, _) = await Client.ResolveDagCidAsync<KuboNomadEventStreamEntry>(cid, nocache: !UseCache, token);
        Guard.IsNotNull(streamEntry);
        return streamEntry;
    }
}