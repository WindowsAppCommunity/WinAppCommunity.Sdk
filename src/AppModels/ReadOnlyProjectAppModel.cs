using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.ComponentModel.Nomad;
using OwlCore.Kubo;
using OwlCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WinAppCommunity.Sdk.Models;
using WinAppCommunity.Sdk.Nomad;
using WinAppCommunity.Sdk.Nomad.Extensions;
using WinAppCommunity.Sdk.Nomad.Kubo;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Creates a new instance of <see cref="ReadOnlyProjectAppModel"/>.
/// </summary>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{T}.TryAdvanceEventStreamAsync"/>. </param>
public class ReadOnlyProjectAppModel(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ReadOnlyProjectNomadKuboEventStreamHandler(listeningEventStreamHandlers)
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
    /// Gets the her image file for this user.
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

    public async IAsyncEnumerable<(ReadOnlyUserAppModel User, Role Role)> GetCollaboratorsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var collaborator in Inner.Collaborators)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<User>(collaborator.User, nocache: !UseCache, cancellationToken);
            Guard.IsNotNull(result);

            var appModel = new ReadOnlyUserAppModel(ListeningEventStreamHandlers)
            {
                Client = Client,
                Id = Inner.Publisher, // assuming project cid is ipns and won't change
                Sources = Sources,
                UseCache = UseCache,
                Inner = result,
            };

            await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow, ContentPointerToStreamEntryAsync, cancellationToken)
            .ToListAsync(cancellationToken);

            yield return (appModel, collaborator.Role);
        }
    }

    /// <summary>
    /// Get the publisher for this project.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async Task<ReadOnlyPublisherAppModel> GetPublisherAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var (result, _) = await Client.ResolveDagCidAsync<Publisher>(Inner.Publisher, nocache: !UseCache, cancellationToken);
        Guard.IsNotNull(result);

        var appModel = new ReadOnlyPublisherAppModel(ListeningEventStreamHandlers)
        {
            Client = Client,
            Id = Inner.Publisher, // assuming project cid is ipns and won't change
            Sources = Sources,
            UseCache = UseCache,
            Inner = result,
        };

        await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow, ContentPointerToStreamEntryAsync, cancellationToken)
            .ToListAsync(cancellationToken);

        return appModel;
    }

    /// <summary>
    /// Get the child publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<ReadOnlyProjectAppModel> GetDependenciesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var projectCid in Inner.Dependencies)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<Project>(projectCid, nocache: !UseCache, cancellationToken);
            Guard.IsNotNull(result);

            var appModel = new ReadOnlyProjectAppModel(ListeningEventStreamHandlers)
            {
                Client = Client,
                Id = projectCid, // assuming project cid is ipns and won't change
                Sources = Sources,
                UseCache = UseCache,
                Inner = result,
            };

            await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow, ContentPointerToStreamEntryAsync, cancellationToken)
                .ToListAsync(cancellationToken);

            yield return appModel;
        }
    }

    private async Task<KuboNomadEventStreamEntry> ContentPointerToStreamEntryAsync(Cid cid, CancellationToken token)
    {
        var (streamEntry, _) = await Client.ResolveDagCidAsync<KuboNomadEventStreamEntry>(cid, nocache: !UseCache, token);
        Guard.IsNotNull(streamEntry);
        return streamEntry;
    }
}