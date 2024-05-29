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
/// Creates a new instance of <see cref="ReadOnlyProject"/>.
/// </summary>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>. </param>
public class ReadOnlyProject(
    ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>>
        listeningEventStreamHandlers)
    : ReadOnlyProjectNomadKuboEventStreamHandler(listeningEventStreamHandlers), IReadOnlyProject
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
            var (result, _) = await Client.ResolveDagCidAsync<User>(collaborator.User, nocache: !KuboOptions.UseCache,
                cancellationToken);
            Guard.IsNotNull(result);

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
                    Inner = result,
                    LocalEventStreamKeyName = LocalEventStreamKeyName,
                    RoamingKeyName = existingKey.Name,
                };

                await appModel.AdvanceEventStreamToAtLeastAsync(EventStreamPosition?.TimestampUtc ?? DateTime.UtcNow,
                    (cid, ct) =>
                        NomadKuboEventStreamHandlerExtensions.ContentPointerToStreamEntryAsync(cid, Client,
                            KuboOptions.UseCache, ct), cancellationToken).ToListAsync(cancellationToken);
                yield return (appModel, collaborator.Role);
            }
            // If current node has no write permissions
            else
            {
                var appModel = new ReadOnlyUser(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    Inner = result,
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
        var (result, _) =
            await Client.ResolveDagCidAsync<Publisher>(Inner.Publisher, nocache: !KuboOptions.UseCache,
                cancellationToken);
        Guard.IsNotNull(result);

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
                Inner = result,
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
                Inner = result,
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
    public async IAsyncEnumerable<IReadOnlyProject> GetDependenciesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var existingKeysEnumerable = await Client.Key.ListAsync(cancellationToken);
        var existingKeys = existingKeysEnumerable.ToOrAsList();

        foreach (var projectCid in Inner.Dependencies)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) =
                await Client.ResolveDagCidAsync<Project>(projectCid, nocache: !KuboOptions.UseCache, cancellationToken);
            Guard.IsNotNull(result);

            // assuming cid is ipns and won't change
            var ipnsId = Inner.Publisher;

            // If current node has write permissions
            if (existingKeys.FirstOrDefault(x => x.Id == ipnsId) is { } existingKey)
            {
                var appModel = new ModifiableProject(ListeningEventStreamHandlers)
                {
                    Client = Client,
                    Id = ipnsId,
                    Sources = Sources,
                    KuboOptions = KuboOptions,
                    Inner = result,
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
                    Inner = result,
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
}