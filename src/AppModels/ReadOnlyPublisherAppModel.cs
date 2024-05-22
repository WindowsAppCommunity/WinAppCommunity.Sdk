﻿using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.Kubo;
using OwlCore.Storage;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Nomad;
using WinAppCommunity.Sdk.Models;
using WinAppCommunity.Sdk.Nomad;
using WinAppCommunity.Sdk.Nomad.Kubo;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Creates a new instance of <see cref="ModifiablePublisherAppModel"/>.
/// </summary>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>. </param>
public class ReadOnlyPublisherAppModel(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ReadOnlyPublisherNomadKuboEventStreamHandler(listeningEventStreamHandlers), IReadOnlyPublisher
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
    public async IAsyncEnumerable<ReadOnlyProjectAppModel> GetProjectsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var projectCid in Inner.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<Project>(projectCid, nocache: !KuboOptions.UseCache, cancellationToken);
            Guard.IsNotNull(result);

            yield return new ReadOnlyProjectAppModel(ListeningEventStreamHandlers)
            {
                Client = Client,
                Id = projectCid, // assuming project cid is ipns and won't change
                Sources = Sources,
                KuboOptions = KuboOptions,
                Inner = result,
            };
        }
    }

    /// <summary>
    /// Get the child publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<ReadOnlyPublisherAppModel> GetChildPublishersAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var publisherCid in Inner.ChildPublishers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<Publisher>(publisherCid, nocache: !KuboOptions.UseCache, cancellationToken);
            Guard.IsNotNull(result);

            yield return new ReadOnlyPublisherAppModel(ListeningEventStreamHandlers)
            {
                Client = Client,
                Id = publisherCid, // assuming publisher cid is ipns and won't change
                Sources = Sources,
                KuboOptions = KuboOptions,
                Inner = result,
            };
        }
    }

    /// <summary>
    /// Get the parent publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<ReadOnlyPublisherAppModel> GetParentPublishersAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var publisherCid in Inner.ParentPublishers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<Publisher>(publisherCid, nocache: !KuboOptions.UseCache, cancellationToken);
            Guard.IsNotNull(result);

            yield return new ReadOnlyPublisherAppModel(ListeningEventStreamHandlers)
            {
                Client = Client,
                Id = publisherCid, // assuming publisher cid is ipns and won't change
                Sources = Sources,
                KuboOptions = KuboOptions,
                Inner = result,
            };
        }
    }
}